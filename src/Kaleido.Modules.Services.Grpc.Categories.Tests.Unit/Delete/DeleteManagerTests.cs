using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;
using Grpc.Core;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Delete
{
    public class DeleteManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly DeleteManager _sut;
        private readonly Guid _categoryKey;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _categoryEntity;

        public DeleteManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<DeleteManager>();

            _categoryKey = Guid.NewGuid();
            _categoryEntity = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "Test Category" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
            };

            _mocker.Use(() =>
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<CategoryMappingProfile>();
                });
                return mapper.CreateMapper();
            });

            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.DeleteAsync(_categoryKey, It.IsAny<BaseRevisionEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntity);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldCallRepositoryDeleteAsync()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.DeleteCategoryAsync(key);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.DeleteAsync(_categoryKey, It.IsAny<BaseRevisionEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnDeletedEntity()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.DeleteCategoryAsync(key);

            // Assert
            Assert.Equal(_categoryEntity, result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.DeleteAsync(_categoryKey, It.IsAny<BaseRevisionEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>)null!);

            // Act
            var result = await _sut.DeleteCategoryAsync(key);

            // Assert
            Assert.Null(result);
        }

    }
}

