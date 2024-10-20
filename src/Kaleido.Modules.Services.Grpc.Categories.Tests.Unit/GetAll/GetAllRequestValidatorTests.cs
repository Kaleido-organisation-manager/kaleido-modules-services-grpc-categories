using Xunit;
using Moq.AutoMock;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAll
{
    public class GetAllRequestValidatorTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllRequestValidator _sut;

        public GetAllRequestValidatorTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllRequestValidator>();
        }

        [Fact]
        public async Task ValidateAsync_ValidRequest_ReturnsValidResult()
        {
            // Arrange
            var request = new GetAllCategoriesRequest();

            // Act
            var result = await _sut.ValidateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_NullRequest_ReturnsValidResult()
        {
            // Arrange
            GetAllCategoriesRequest request = null!;

            // Act
            var result = await _sut.ValidateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_WithCancellationToken_ReturnsValidResult()
        {
            // Arrange
            var request = new GetAllCategoriesRequest();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _sut.ValidateAsync(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}

