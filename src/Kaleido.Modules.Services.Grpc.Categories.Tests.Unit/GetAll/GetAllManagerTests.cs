using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAll
{
    public class GetAllManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllManager _sut;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _categoryEntities;

        public GetAllManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllManager>();

            _categoryEntities = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 2" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };

            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAllAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntities);
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallRepositoryGetAllActiveAsync()
        {
            // Act
            await _sut.GetAllAsync();

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAllAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyListWhenNoCategories()
        {
            // Arrange
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAllAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldPassCancellationTokenToHandler()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetAllAsync(cancellationToken);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAllAsync(It.IsAny<Guid?>(), cancellationToken), Times.Once);
        }
    }
}

