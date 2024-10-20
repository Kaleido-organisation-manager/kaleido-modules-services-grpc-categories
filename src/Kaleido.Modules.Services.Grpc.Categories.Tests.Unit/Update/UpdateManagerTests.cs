using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly UpdateManager _sut;
        private readonly Category _validCategory;
        private readonly CategoryEntity _storedCategory;
        private readonly CategoryEntity _updatedCategoryEntity;

        public UpdateManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<UpdateManager>();

            _validCategory = new Category
            {
                Key = Guid.NewGuid().ToString(),
                Name = "Updated Category Name"
            };

            _storedCategory = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Key = Guid.Parse(_validCategory.Key),
                Name = "Original Category Name",
                Revision = 1,
                Status = Kaleido.Common.Services.Grpc.Constants.EntityStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _updatedCategoryEntity = new CategoryEntity
            {
                Id = _storedCategory.Id,
                Key = _storedCategory.Key,
                Name = _validCategory.Name,
                Revision = _storedCategory.Revision + 1,
                Status = _storedCategory.Status,
                CreatedAt = DateTime.UtcNow
            };

            // Happy path setup
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_storedCategory);

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.UpdateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_updatedCategoryEntity);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToEntity(It.IsAny<Category>(), It.IsAny<int>()))
                .Returns(_updatedCategoryEntity);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategory(It.IsAny<CategoryEntity>()))
                .Returns(_validCategory);
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_ReturnsUpdatedCategory()
        {
            // Act
            var result = await _sut.UpdateAsync(_validCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_validCategory.Key, result.Key);
            Assert.Equal(_validCategory.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_CallsGetActiveAsync()
        {
            // Act
            await _sut.UpdateAsync(_validCategory);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetActiveAsync(Guid.Parse(_validCategory.Key), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_CallsMapperToEntity()
        {
            // Act
            await _sut.UpdateAsync(_validCategory);

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToEntity(_validCategory, _storedCategory.Revision + 1), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_CallsUpdateAsync()
        {
            // Act
            await _sut.UpdateAsync(_validCategory);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.UpdateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_CallsMapperToCategory()
        {
            // Act
            await _sut.UpdateAsync(_validCategory);

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToCategory(_updatedCategoryEntity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CategoryNotFound_ReturnsNull()
        {
            // Arrange
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetActiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryEntity)null!);

            // Act
            var result = await _sut.UpdateAsync(_validCategory);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_PassesCancellationToken()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.UpdateAsync(_validCategory, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetActiveAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);

            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.UpdateAsync(It.IsAny<CategoryEntity>(), cancellationToken), Times.Once);
        }
    }
}

