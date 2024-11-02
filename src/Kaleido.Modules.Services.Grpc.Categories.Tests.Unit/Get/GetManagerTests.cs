using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Get
{
    public class GetManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetManager _sut;
        private readonly Guid _categoryKey;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _categoryEntity;
        private readonly Category _category;

        public GetManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetManager>();

            _categoryKey = Guid.NewGuid();
            _categoryEntity = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "Test Category" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid(), Key = _categoryKey }
            };
            _category = new Category { Name = "Test Category" };

            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAsync(_categoryKey, It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntity);
        }

        [Fact]
        public async Task GetAsync_ShouldCallRepositoryGetActiveAsync()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetAsync(key);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAsync(_categoryKey, It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMappedCategory()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.GetAsync(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_category.Name, result.Entity.Name);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Setup(r => r.GetAsync(_categoryKey, It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>)null!);

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
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAsync(_categoryKey, It.IsAny<int?>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.GetAsync(key);

            // Assert
            _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
                .Verify(r => r.GetAsync(_categoryKey, It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

