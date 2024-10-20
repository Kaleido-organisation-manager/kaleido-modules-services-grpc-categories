using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Exists;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Exists
{
    public class ExistsManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly ExistsManager _sut;
        private readonly string _validKey;

        public ExistsManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ExistsManager>();
            _validKey = Guid.NewGuid().ToString();

            // Happy path setup
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task ExistsAsync_ValidKey_CallsRepositoryWithParsedGuid()
        {
            // Act
            await _sut.ExistsAsync(_validKey);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.ExistsAsync(Guid.Parse(_validKey), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ValidKey_ReturnsRepositoryResult()
        {
            // Act
            var result = await _sut.ExistsAsync(_validKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_RepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.ExistsAsync(_validKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_PassesCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.ExistsAsync(_validKey, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.ExistsAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_InvalidGuid_ThrowsFormatException()
        {
            // Arrange
            var invalidKey = "invalid-guid";

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(() => _sut.ExistsAsync(invalidKey));
        }
    }
}

