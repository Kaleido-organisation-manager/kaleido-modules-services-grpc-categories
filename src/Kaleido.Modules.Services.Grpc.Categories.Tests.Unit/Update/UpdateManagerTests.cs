using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly UpdateManager _sut;
        private readonly CategoryEntity _categoryEntity;
        private readonly Guid _categoryKey;

        public UpdateManagerTests()
        {
            _mocker = new AutoMocker();

            _categoryKey = Guid.NewGuid();
            _categoryEntity = new CategoryEntity
            {
                Id = _categoryKey,
                Name = "Test Category"
            };



            _sut = _mocker.CreateInstance<UpdateManager>();
        }

        [Fact]
        public async Task UpdateAsync_ValidCategory_ReturnsUpdatedEntity()
        {
            // Arrange
            var expectedResult = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = _categoryEntity,
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
            };

            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(handler => handler.UpdateAsync(_categoryKey, _categoryEntity, It.IsAny<BaseRevisionEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _sut.UpdateAsync(_categoryKey, _categoryEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Entity, result.Entity);
        }


        [Fact]
        public async Task UpdateAsync_ManagerThrowsRevisionNotFoundException_ReturnsNull()
        {
            // Arrange
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(handler => handler.UpdateAsync(_categoryKey, _categoryEntity, It.IsAny<BaseRevisionEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RevisionNotFoundException("Test exception"));

            // Act
            var result = await _sut.UpdateAsync(_categoryKey, _categoryEntity);

            // Assert
            Assert.Null(result);
        }

    }
}

