using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Kaleido.Common.Services.Grpc.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly UpdateHandler _sut;
        private readonly UpdateCategoryRequest _validRequest;
        private readonly Category _updatedCategory;

        public UpdateHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<UpdateHandler>();

            var categoryKey = Guid.NewGuid().ToString();
            _validRequest = new UpdateCategoryRequest
            {
                Key = categoryKey,
                Category = new Category { Key = categoryKey, Name = "Updated Category" }
            };

            _updatedCategory = new Category { Key = categoryKey, Name = "Updated Category" };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<UpdateCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_updatedCategory);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsUpdateCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UpdateCategoryResponse>(result);
            Assert.Equal(_updatedCategory, result.Category);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<UpdateCategoryRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IUpdateManager>()
                .Verify(m => m.UpdateAsync(_validRequest.Category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name format");
            _mocker.GetMock<IRequestValidator<UpdateCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

