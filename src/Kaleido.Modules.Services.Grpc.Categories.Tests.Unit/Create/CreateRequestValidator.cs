using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Create;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Create
{
    public class CreateRequestValidatorTests
    {
        private readonly AutoMocker _mocker;
        private readonly CreateRequestValidator _sut;
        private readonly CreateCategoryRequest _validRequest;

        public CreateRequestValidatorTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CreateRequestValidator>();

            _validRequest = new CreateCategoryRequest
            {
                Category = new CreateCategory { Name = "Test Category" }
            };

            // Happy path setup
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateCreateAsync(It.IsAny<CreateCategory>(), It.IsAny<CancellationToken>()))
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
                .Verify(v => v.ValidateCreateAsync(_validRequest.Category, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateAsync_CategoryValidatorReturnsErrors_ReturnsValidationResultWithErrors()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name");
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateCreateAsync(It.IsAny<CreateCategory>(), It.IsAny<CancellationToken>()))
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
            var invalidRequest = new CreateCategoryRequest { Category = null };

            // Act & Assert
            var result = await _sut.ValidateAsync(invalidRequest);

            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Single(result.Errors);
        }
    }
}

