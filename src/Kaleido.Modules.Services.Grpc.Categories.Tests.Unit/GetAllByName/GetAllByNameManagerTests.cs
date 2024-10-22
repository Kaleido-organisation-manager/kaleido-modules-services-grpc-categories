using Xunit;
using Moq;
using Moq.AutoMock;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllByNameManagerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllByNameManager _sut;
        private readonly string _testName;
        private readonly List<CategoryEntity> _testCategories;
        private readonly List<Category> _mappedCategories;

        public GetAllByNameManagerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllByNameManager>();

            _testName = "Test Category";
            _testCategories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = Guid.NewGuid(), Key = Guid.NewGuid(), Name = "Test Category 1" },
                new CategoryEntity { Id = Guid.NewGuid(), Key = Guid.NewGuid(), Name = "Test Category 2" }
            };
            _mappedCategories = new List<Category>
            {
                new Category { Key = _testCategories[0].Key.ToString(), Name = _testCategories[0].Name },
                new Category { Key = _testCategories[1].Key.ToString(), Name = _testCategories[1].Name }
            };

            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_testCategories);

            _mocker.GetMock<ICategoryMapper>()
                .Setup(m => m.ToCategory(It.IsAny<CategoryEntity>()))
                .Returns<CategoryEntity>(ce => new Category { Key = ce.Key.ToString(), Name = ce.Name });
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldCallRepositoryWithCorrectName()
        {
            // Act
            await _sut.GetAllByNameAsync(_testName);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllByNameAsync(_testName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldMapAllReturnedCategories()
        {
            // Act
            await _sut.GetAllByNameAsync(_testName);

            // Assert
            _mocker.GetMock<ICategoryMapper>()
                .Verify(m => m.ToCategory(It.IsAny<CategoryEntity>()), Times.Exactly(_testCategories.Count));
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldReturnMappedCategories()
        {
            // Act
            var result = await _sut.GetAllByNameAsync(_testName);

            // Assert
            Assert.Equal(_mappedCategories.Count, result.Count());
            Assert.All(result, c => Assert.Contains(c, _mappedCategories));
        }

        [Fact]
        public async Task GetAllByNameAsync_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            // Act
            await _sut.GetAllByNameAsync(_testName, cancellationToken);

            // Assert
            _mocker.GetMock<ICategoryRepository>()
                .Verify(r => r.GetAllByNameAsync(_testName, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAllByNameAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            _mocker.GetMock<ICategoryRepository>()
                .Setup(r => r.GetAllByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryEntity>());

            // Act
            var result = await _sut.GetAllByNameAsync(_testName);

            // Assert
            Assert.Empty(result);
        }
    }
}

