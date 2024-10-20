using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetRevision
{
    public class GetRevisionManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetRevisionManager _sut;
        private readonly Guid _categoryKey;
        private readonly int _revision;
        private readonly CategoryEntity _categoryEntity;
        private readonly CategoryRevision _categoryRevision;

        public GetRevisionManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetRevisionManager>();

            _categoryKey = Guid.NewGuid();
            _revision = 1;
            _categoryEntity = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Key = _categoryKey,
                Name = "Test Category",
                Revision = _revision,
                Status = Kaleido.Common.Services.Grpc.Constants.EntityStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            _categoryRevision = new CategoryRevision
            {
                Key = _categoryKey.ToString(),
                Name = "Test Category",
                Revision = _revision,
                Status = "Active",
                CreatedAt = _categoryEntity.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            // Happy path setup
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetRevisionAsync(_categoryKey, _revision, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntity);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategoryRevision(_categoryEntity))
                .Returns(_categoryRevision);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetRevisionAsync(key, _revision);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetRevisionAsync(_categoryKey, _revision, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldReturnMappedCategoryRevision()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.GetRevisionAsync(key, _revision);

            // Assert
            Assert.Equal(_categoryRevision, result);
        }

        [Fact]
        public async Task GetRevisionAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetRevisionAsync(_categoryKey, _revision, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryEntity)null!);

            // Act
            var result = await _sut.GetRevisionAsync(key, _revision);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var key = _categoryKey.ToString();
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetRevisionAsync(key, _revision, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetRevisionAsync(_categoryKey, _revision, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetRevisionAsync(key, _revision);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetRevisionAsync(_categoryKey, _revision, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

