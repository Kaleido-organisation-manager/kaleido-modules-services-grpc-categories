using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Microsoft.Extensions.Logging;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Get
{
    public class GetHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetHandler _sut;
        private readonly GetCategoryRequest _validRequest;
        private readonly Category _validCategory;

        public GetHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetHandler>();

            _validRequest = new GetCategoryRequest { Key = "valid-key" };
            _validCategory = new Category { Key = "valid-key", Name = "Test Category" };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<GetCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validCategory);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetCategoryResponse>(result);
            Assert.Equal(_validCategory, result.Category);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<GetCategoryRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IGetManager>()
                .Verify(m => m.GetAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");
            _mocker.GetMock<IRequestValidator<GetCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

