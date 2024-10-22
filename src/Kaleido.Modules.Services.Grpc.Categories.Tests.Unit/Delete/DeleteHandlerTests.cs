using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Delete
{
    public class DeleteHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly DeleteHandler _sut;
        private readonly DeleteCategoryRequest _validRequest;

        public DeleteHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<DeleteHandler>();

            var categoryKey = Guid.NewGuid();

            _validRequest = new DeleteCategoryRequest
            {
                Key = categoryKey.ToString()
            };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<DeleteCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<DeleteCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CategoryEntity { Key = categoryKey, Name = "valid-name" });
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsDeleteCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DeleteCategoryResponse>(result);
            Assert.Equal(_validRequest.Key, result.Key);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<DeleteCategoryRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IDeleteManager>()
                .Verify(m => m.DeleteCategoryAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");
            _mocker.GetMock<IRequestValidator<DeleteCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<DeleteCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryEntity?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

