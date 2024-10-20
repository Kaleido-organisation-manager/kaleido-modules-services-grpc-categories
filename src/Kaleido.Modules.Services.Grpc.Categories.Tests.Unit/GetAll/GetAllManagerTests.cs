using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAll
{
    public class GetAllManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllManager _sut;
        private readonly List<CategoryEntity> _categoryEntities;
        private readonly List<Category> _categories;

        public GetAllManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllManager>();

            _categoryEntities = new List<CategoryEntity>
            {
                new CategoryEntity { Id = Guid.NewGuid(), Key = Guid.NewGuid(), Name = "Category 1" },
                new CategoryEntity { Id = Guid.NewGuid(), Key = Guid.NewGuid(), Name = "Category 2" }
            };

            _categories = new List<Category>
            {
                new Category { Key = _categoryEntities[0].Key.ToString(), Name = "Category 1" },
                new Category { Key = _categoryEntities[1].Key.ToString(), Name = "Category 2" }
            };

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntities);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategory(It.IsAny<CategoryEntity>()))
                .Returns<CategoryEntity>(entity => _categories.First(c => c.Key == entity.Key.ToString()));
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallRepositoryGetAllActiveAsync()
        {
            // Act
            await _sut.GetAllAsync();

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallMapperToCategory()
        {
            // Act
            await _sut.GetAllAsync();

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToCategory(It.IsAny<CategoryEntity>()), Times.Exactly(_categoryEntities.Count));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedCategories()
        {
            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_categories.Count, result.Count());
            Assert.Equal(_categories, result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyListWhenNoCategories()
        {
            // Arrange
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryEntity>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetAllAsync(cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllActiveAsync(cancellationToken), Times.Once);
        }
    }
}

