using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Get
{
    public class GetManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetManager _sut;
        private readonly Guid _categoryKey;
        private readonly CategoryEntity _categoryEntity;
        private readonly Category _category;

        public GetManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetManager>();

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
            _category = new Category { Key = _categoryKey.ToString(), Name = "Test Category" };

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetActiveAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntity);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategory(_categoryEntity))
                .Returns(_category);
        }

        [Fact]
        public async Task GetAsync_ShouldCallRepositoryGetActiveAsync()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetAsync(key);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetActiveAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldCallMapperToCategory()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetAsync(key);

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToCategory(_categoryEntity), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMappedCategory()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.GetAsync(key);

            // Assert
            Assert.Equal(_category, result);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetActiveAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryEntity)null!);

            // Act
            var result = await _sut.GetAsync(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var key = _categoryKey.ToString();
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetAsync(key, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetActiveAsync(_categoryKey, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetAsync(key);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetActiveAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

