using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllRevisions
{
    public class GetAllRevisionsManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllRevisionsManager _sut;
        private readonly Guid _categoryKey;
        private readonly List<CategoryEntity> _categoryEntities;
        private readonly List<CategoryRevision> _categoryRevisions;

        public GetAllRevisionsManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllRevisionsManager>();

            _categoryKey = Guid.NewGuid();
            _categoryEntities = new List<CategoryEntity>
            {
                new CategoryEntity { Id = Guid.NewGuid(), Key = _categoryKey, Name = "Category 1", Revision = 1 },
                new CategoryEntity { Id = Guid.NewGuid(), Key = _categoryKey, Name = "Category 1 Updated", Revision = 2 }
            };
            _categoryRevisions = new List<CategoryRevision>
            {
                new CategoryRevision { Key = _categoryKey.ToString(), Name = "Category 1", Revision = 1 },
                new CategoryRevision { Key = _categoryKey.ToString(), Name = "Category 1 Updated", Revision = 2 }
            };

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllRevisionsAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categoryEntities);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategoryRevision(It.IsAny<CategoryEntity>()))
                .Returns<CategoryEntity>(entity => _categoryRevisions.First(r => r.Revision == entity.Revision));
        }

        [Fact]
        public async Task HandleAsync_ShouldCallRepositoryGetAllRevisionsAsync()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.HandleAsync(key);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllRevisionsAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldCallMapperToCategoryRevisionForEachEntity()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.HandleAsync(key);

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToCategoryRevision(It.IsAny<CategoryEntity>()), Times.Exactly(_categoryEntities.Count));
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnMappedCategoryRevisions()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            var result = await _sut.HandleAsync(key);

            // Assert
            Assert.Equal(_categoryRevisions.Count, result.Count());
            Assert.Equal(_categoryRevisions, result);
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
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllRevisionsAsync(_categoryKey, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldParseKeyCorrectly()
        {
            // Arrange
            var key = _categoryKey.ToString();

            // Act
            await _sut.HandleAsync(key);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllRevisionsAsync(_categoryKey, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var key = _categoryKey.ToString();
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllRevisionsAsync(_categoryKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryEntity>());

            // Act
            var result = await _sut.HandleAsync(key);

            // Assert
            Assert.Empty(result);
        }
    }
}

