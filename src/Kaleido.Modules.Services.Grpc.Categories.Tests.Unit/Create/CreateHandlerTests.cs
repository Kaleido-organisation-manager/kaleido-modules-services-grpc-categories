using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Microsoft.Extensions.Logging;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Create
{
    public class CreateHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly CreateHandler _sut;
        private readonly CreateCategoryRequest _validRequest;

        public CreateHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CreateHandler>();

            _validRequest = new CreateCategoryRequest
            {
                Category = new CreateCategory { Name = "Test Category" }
            };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<CreateCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<ICreateManager>()
                .Setup(m => m.CreateAsync(It.IsAny<CreateCategory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Category { Key = "test-key", Name = "Test Category" });
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsCreateCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CreateCategoryResponse>(result);
            Assert.Equal("Test Category", result.Category.Name);
            Assert.Equal("test-key", result.Category.Key);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<CreateCategoryRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<ICreateManager>()
                .Verify(m => m.CreateAsync(_validRequest.Category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsRpcException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name");
            _mocker.GetMock<IRequestValidator<CreateCategoryRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<ICreateManager>()
                .Setup(m => m.CreateAsync(It.IsAny<CreateCategory>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

