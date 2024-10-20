using Moq;
using Moq.AutoMock;
using Xunit;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Constants;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Create;

public class CreateManagerTests
{
    private readonly AutoMocker _mocker;
    private readonly CreateManager _sut;
    private readonly CreateCategory _createCategory;
    private readonly CategoryEntity _categoryEntity;
    private readonly Category _category;

    public CreateManagerTests()
    {
        _mocker = new AutoMocker();
        _sut = _mocker.CreateInstance<CreateManager>();

        _createCategory = new CreateCategory { Name = "Test Category" };
        _categoryEntity = new CategoryEntity
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Name = "Test Category",
            CreatedAt = DateTime.UtcNow,
            Revision = 1,
            Status = EntityStatus.Active
        };
        _category = new Category { Key = _categoryEntity.Key.ToString(), Name = _categoryEntity.Name };

        _mocker.GetMock<ICategoryMapper>()
            .Setup(m => m.ToCreateEntity(_createCategory))
            .Returns(_categoryEntity);

        _mocker.GetMock<ICategoryMapper>()
            .Setup(m => m.ToCategory(_categoryEntity))
            .Returns(_category);

        _mocker.GetMock<ICategoryRepository>()
            .Setup(r => r.CreateAsync(_categoryEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_categoryEntity);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallMapperToCreateEntity()
    {
        // Act
        await _sut.CreateAsync(_createCategory);

        // Assert
        _mocker.GetMock<ICategoryMapper>()
            .Verify(m => m.ToCreateEntity(_createCategory), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryCreateAsync()
    {
        // Act
        await _sut.CreateAsync(_createCategory);

        // Assert
        _mocker.GetMock<ICategoryRepository>()
            .Verify(r => r.CreateAsync(_categoryEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallMapperToCategory()
    {
        // Act
        await _sut.CreateAsync(_createCategory);

        // Assert
        _mocker.GetMock<ICategoryMapper>()
            .Verify(m => m.ToCategory(_categoryEntity), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnMappedCategory()
    {
        // Act
        var result = await _sut.CreateAsync(_createCategory);

        // Assert
        Assert.Equal(_category, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.CreateAsync(_createCategory, cancellationToken);

        // Assert
        _mocker.GetMock<ICategoryRepository>()
            .Verify(r => r.CreateAsync(_categoryEntity, cancellationToken), Times.Once);
    }
}

