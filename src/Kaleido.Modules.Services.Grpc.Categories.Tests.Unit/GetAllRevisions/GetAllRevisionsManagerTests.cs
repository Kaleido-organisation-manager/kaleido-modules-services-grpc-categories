using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllRevisions
{
    public class GetAllRevisionsManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllRevisionsManager _sut;
        private readonly Guid _categoryKey;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _categoryEntities;

        public GetAllRevisionsManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllRevisionsManager>();

            _categoryKey = Guid.NewGuid();
            _categoryEntities = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 1 Updated" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };

            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAllAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntities);
        }

        [Fact]
        public async Task HandleAsync_ShouldCallRepositoryGetAllRevisionsAsync()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.HandleAsync(key);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAllAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task HandleAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var key = _categoryKey.ToString();
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.HandleAsync(key, cancellationToken);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAllAsync(_categoryKey, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.HandleAsync(key);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAllAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAllAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>());

            // Act
            var result = await _sut.HandleAsync(key);

            // Assert
            Assert.Empty(result);
        }
    }
}

