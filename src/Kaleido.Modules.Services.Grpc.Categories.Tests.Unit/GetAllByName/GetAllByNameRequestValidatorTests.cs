using Xunit;
using Moq.AutoMock;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Moq;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllByNameRequestValidatorTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllByNameRequestValidator _sut;
        private readonly GetAllCategoriesByNameRequest _validRequest;

        public GetAllByNameRequestValidatorTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllByNameRequestValidator>();

            _validRequest = new GetAllCategoriesByNameRequest { Name = "Valid Category Name" };

            // Happy path setup
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateCategoryNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
        }

        [Fact]
        public async Task ValidateAsync_ValidRequest_ReturnsValidResult()
        {
            // Act
            var result = await _sut.ValidateAsync(_validRequest);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_CallsCategoryValidator()
        {
            // Act
            await _sut.ValidateAsync(_validRequest);

            // Assert
            _mocker.GetMock<ICategoryValidator>()
                .Verify(v => v.ValidateCategoryNameAsync(_validRequest.Name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateAsync_InvalidName_ReturnsInvalidResult()
        {
            // Arrange
            var invalidRequest = new GetAllCategoriesByNameRequest { Name = "Invalid Name" };
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name format");

            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateCategoryNameAsync(invalidRequest.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateAsync(invalidRequest);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_NullRequest_ReturnsInvalidResult()
        {
            // Act
            var result = await _sut.ValidateAsync(null!);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_EmptyName_ReturnsInvalidResult()
        {
            // Arrange
            var requestWithEmptyName = new GetAllCategoriesByNameRequest { Name = string.Empty };
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Name is required");

            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateCategoryNameAsync(string.Empty, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateAsync(requestWithEmptyName);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }
    }
}

