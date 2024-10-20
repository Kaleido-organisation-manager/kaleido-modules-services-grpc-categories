using Xunit;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Common.Services.Grpc.Constants;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Mappers;

public class CategoryMapperTests
{
    private readonly AutoMocker _mocker;
    private readonly CategoryMapper _sut;
    private readonly CategoryEntity _categoryEntity;
    private readonly Category _category;
    private readonly CreateCategory _createCategory;

    public CategoryMapperTests()
    {
        _mocker = new AutoMocker();
        _sut = _mocker.CreateInstance<CategoryMapper>();

        // Set up test data
        _categoryEntity = new CategoryEntity
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Name = "Test Category",
            Revision = 1,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _category = new Category
        {
            Key = Guid.NewGuid().ToString(),
            Name = "Test Category"
        };

        _createCategory = new CreateCategory
        {
            Name = "New Test Category"
        };
    }

    [Fact]
    public void ToCategory_ShouldMapCorrectly()
    {
        // Act
        var result = _sut.ToCategory(_categoryEntity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_categoryEntity.Key.ToString(), result.Key);
        Assert.Equal(_categoryEntity.Name, result.Name);
    }

    [Fact]
    public void ToCategoryRevision_ShouldMapCorrectly()
    {
        // Act
        var result = _sut.ToCategoryRevision(_categoryEntity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_categoryEntity.Key.ToString(), result.Key);
        Assert.Equal(_categoryEntity.Name, result.Name);
        Assert.Equal(_categoryEntity.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"), result.CreatedAt);
        Assert.Equal(_categoryEntity.Revision, result.Revision);
        Assert.Equal(_categoryEntity.Status.ToString(), result.Status);
    }

    [Fact]
    public void ToCreateEntity_ShouldMapCorrectly()
    {
        // Act
        var result = _sut.ToCreateEntity(_createCategory);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(Guid.Empty, result.Key);
        Assert.Equal(_createCategory.Name, result.Name);
        Assert.Equal(1, result.Revision);
        Assert.Equal(EntityStatus.Active, result.Status);
        Assert.True((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void ToEntity_ShouldMapCorrectly()
    {
        // Act
        var result = _sut.ToEntity(_category);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(Guid.Parse(_category.Key), result.Key);
        Assert.Equal(_category.Name, result.Name);
        Assert.Equal(1, result.Revision);
        Assert.Equal(EntityStatus.Active, result.Status);
        Assert.True((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void ToEntity_WithNewRevision_ShouldMapCorrectly()
    {
        // Arrange
        int newRevision = 5;

        // Act
        var result = _sut.ToEntity(_category, newRevision);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(Guid.Parse(_category.Key), result.Key);
        Assert.Equal(_category.Name, result.Name);
        Assert.Equal(newRevision, result.Revision);
        Assert.Equal(EntityStatus.Active, result.Status);
        Assert.True((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 1);
    }
}
