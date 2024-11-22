using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetRevision
{
    public class GetRevisionManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetRevisionManager _sut;
        private readonly Guid _categoryKey;
        private readonly DateTime _createdAt;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _categoryEntity;

        public GetRevisionManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetRevisionManager>();

            _categoryKey = Guid.NewGuid();
            _createdAt = DateTime.UtcNow;
            _categoryEntity = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Category",
                },
                Revision = new BaseRevisionEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = _createdAt,
                    Key = _categoryKey
                }
            };

            // Happy path setup
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetHistoricAsync(_categoryKey, _createdAt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntity);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldCallRepositoryWithCorrectParameters()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetRevisionAsync(key, _createdAt);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetHistoricAsync(_categoryKey, _createdAt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldReturnMappedCategoryRevision()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.GetRevisionAsync(key, _createdAt);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_categoryEntity.Key, result.Key);
        }

        [Fact]
        public async Task GetRevisionAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetHistoricAsync(_categoryKey, _createdAt, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>)null!);

            // Act
            var result = await _sut.GetRevisionAsync(key, _createdAt);

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
            await _sut.GetRevisionAsync(key, _createdAt, cancellationToken);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetHistoricAsync(_categoryKey, _createdAt, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetRevisionAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetRevisionAsync(key, _createdAt);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetHistoricAsync(_categoryKey, _createdAt, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

