using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using FluentValidation.TestHelper;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Validators
{
    public class CategoryValidatorTests
    {
        private readonly CategoryValidator _sut;

        public CategoryValidatorTests()
        {
            _sut = new CategoryValidator();
        }

        [Fact]
        public void Validate_ValidCategory_ShouldNotHaveValidationError()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };

            // Act
            var result = _sut.TestValidate(category);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidName_ShouldHaveValidationError(string name)
        {
            // Arrange
            var category = new Category { Name = name };

            // Act
            var result = _sut.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Validate_NameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var category = new Category { Name = new string('a', 256) };

            // Act
            var result = _sut.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
    }
}