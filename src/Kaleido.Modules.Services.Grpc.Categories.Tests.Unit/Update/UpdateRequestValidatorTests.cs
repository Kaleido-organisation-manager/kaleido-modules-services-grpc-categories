using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Update;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateRequestValidatorTests
    {
        private readonly AutoMocker _mocker;
        private readonly UpdateRequestValidator _sut;
        private readonly UpdateCategoryRequest _validRequest;

        public UpdateRequestValidatorTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<UpdateRequestValidator>();

            var categoryKey = Guid.NewGuid().ToString();
            _validRequest = new UpdateCategoryRequest
            {
                Key = categoryKey,
                Category = new Category { Key = categoryKey, Name = "Valid Category" }
            };

            // Happy path setup
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateUpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        [Fact]
        public async Task ValidateAsync_ValidRequest_ReturnsValidationResult()
        {
            // Act
            var result = await _sut.ValidateAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_CallsCategoryValidator()
        {
            // Act
            await _sut.ValidateAsync(_validRequest);

            // Assert
            _mocker.GetMock<ICategoryValidator>()
                .Verify(v => v.ValidateUpdateAsync(_validRequest.Category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateAsync_CategoryValidatorReturnsErrors_ReturnsValidationResultWithErrors()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name");
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateUpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_NullRequest_IsNotValid()
        {
            // Act & Assert
            var result = await _sut.ValidateAsync(null!);

            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_NullCategory_IsNotValid()
        {
            // Arrange
            var invalidRequest = new UpdateCategoryRequest { Category = null };

            // Act & Assert
            var result = await _sut.ValidateAsync(invalidRequest);

            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_PassesCancellationToken()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.ValidateAsync(_validRequest, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryValidator>()
                .Verify(v => v.ValidateUpdateAsync(_validRequest.Category, cancellationToken), Times.Once);
        }
    }
}

