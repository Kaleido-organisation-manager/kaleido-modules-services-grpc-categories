using Moq;
using Moq.AutoMock;
using Xunit;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Delete
{
    public class DeleteManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly DeleteManager _sut;
        private readonly Guid _categoryKey;
        private readonly CategoryEntity _categoryEntity;

        public DeleteManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<DeleteManager>();

            _categoryKey = Guid.NewGuid();
            _categoryEntity = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Key = _categoryKey,
                Name = "Test Category",
                Revision = 1,
                Status = Kaleido.Common.Services.Grpc.Constants.EntityStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.DeleteAsync(_categoryKey, It.IsAny<CancellationToken>()))
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
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.DeleteAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
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
        public async Task DeleteCategoryAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var key = _categoryKey.ToString();
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.DeleteCategoryAsync(key, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.DeleteAsync(_categoryKey, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.DeleteAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryEntity)null!);

            // Act
            var result = await _sut.DeleteCategoryAsync(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.DeleteCategoryAsync(key);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.DeleteAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

