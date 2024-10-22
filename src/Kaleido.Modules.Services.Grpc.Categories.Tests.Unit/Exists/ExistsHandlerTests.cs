using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Exists;
using Microsoft.Extensions.Logging;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Exists
{
    public class ExistsHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly ExistsHandler _sut;
        private readonly CategoryExistsRequest _validRequest;

        public ExistsHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ExistsHandler>();

            _validRequest = new CategoryExistsRequest { Key = "valid-key" };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<CategoryExistsRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<CategoryExistsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IExistsManager>()
                .Setup(m => m.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsCategoryExistsResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryExistsResponse>(result);
            Assert.True(result.Exists);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<CategoryExistsRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IExistsManager>()
                .Verify(m => m.ExistsAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");
            _mocker.GetMock<IRequestValidator<CategoryExistsRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<CategoryExistsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IExistsManager>()
                .Setup(m => m.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsFalse_ReturnsFalseResponse()
        {
            // Arrange
            _mocker.GetMock<IExistsManager>()
                .Setup(m => m.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryExistsResponse>(result);
            Assert.False(result.Exists);
        }
    }
}

