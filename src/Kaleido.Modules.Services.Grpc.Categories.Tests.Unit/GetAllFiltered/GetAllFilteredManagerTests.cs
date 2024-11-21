using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using System.Linq.Expressions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllFilteredManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllFilteredManager _sut;
        private readonly string _testName;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _testCategories;

        public GetAllFilteredManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllFilteredManager>();

            _testName = "Test Category";
            _testCategories = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Test Category 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Test Category 2" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };


            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.FindAllAsync(It.IsAny<Expression<Func<CategoryEntity, bool>>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_testCategories);
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldCallHandler()
        {
            // Act
            await _sut.GetAllByNameAsync(_testName);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.FindAllAsync(It.IsAny<Expression<Func<CategoryEntity, bool>>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldReturnMappedCategories()
        {
            // Act
            var result = await _sut.GetAllByNameAsync(_testName);

            // Assert
            Assert.Equal(_testCategories.Count, result.Count());
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetAllByNameAsync(_testName, cancellationToken);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.FindAllAsync(It.IsAny<Expression<Func<CategoryEntity, bool>>>(), It.IsAny<Guid?>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAllByNameAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.FindAllAsync(It.IsAny<Expression<Func<CategoryEntity, bool>>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>());

            // Act
            var result = await _sut.GetAllByNameAsync(_testName);

            // Assert
            Assert.Empty(result);
        }
    }
}

