using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Get;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Get
{
    public class GetRequestValidatorTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetRequestValidator _sut;
        private readonly GetCategoryRequest _validRequest;

        public GetRequestValidatorTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetRequestValidator>();

            _validRequest = new GetCategoryRequest { Key = "valid-guid" };

            // Happy path setup
            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateKeyFormatAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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
                .Verify(v => v.ValidateKeyFormatAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateAsync_InvalidKey_ReturnsInvalidResult()
        {
            // Arrange
            var invalidRequest = new GetCategoryRequest { Key = "invalid-key" };
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");

            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateKeyFormatAsync(invalidRequest.Key, It.IsAny<CancellationToken>()))
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
        public async Task ValidateAsync_EmptyKey_ReturnsInvalidResult()
        {
            // Arrange
            var requestWithEmptyKey = new GetCategoryRequest { Key = string.Empty };
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Key is required");

            _mocker.GetMock<ICategoryValidator>()
                .Setup(v => v.ValidateKeyFormatAsync(string.Empty, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateAsync(requestWithEmptyKey);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }
    }
}

