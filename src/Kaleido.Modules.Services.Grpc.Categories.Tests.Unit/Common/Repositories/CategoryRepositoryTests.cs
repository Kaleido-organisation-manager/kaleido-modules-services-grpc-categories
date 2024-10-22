using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Tests.Unit.Repositories.Fixture;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Repositories.Builders;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Repositories;

public class CategoryRepositoryTests : IClassFixture<CategoryRepositoryFixture>
{
    private CategoryRepositoryFixture _fixture { get; set; }

    public CategoryRepositoryTests(CategoryRepositoryFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task GetAllByNameAsync_ShouldReturnMatchingActiveCategories()
    {
        // Arrange
        var name = "Test";
        var categories = new List<CategoryEntity>
        {
            new CategoryEntityBuilder().WithName("Test Category").Build(),
            new CategoryEntityBuilder().WithName("Test Category 2").Build()
        };

        _fixture.DbContext.Categories.AddRange(categories);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _fixture.Repository.GetAllByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Contains(name, c.Name, StringComparison.OrdinalIgnoreCase));
        Assert.All(result, c => Assert.Equal(EntityStatus.Active, c.Status));
    }

    [Fact]
    public async Task GetAllByNameAsync_ShouldReturnEmptyListForNonMatchingName()
    {
        // Arrange
        var name = "NonExistent";
        var categories = new List<CategoryEntity>
        {
            new CategoryEntityBuilder().WithName("Test Category").Build()
        };

        _fixture.DbContext.Categories.AddRange(categories);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _fixture.Repository.GetAllByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllByNameAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        var name = "test";
        var categories = new List<CategoryEntity>
        {
            new CategoryEntityBuilder().WithName("Test Category").Build(),
            new CategoryEntityBuilder().WithName("Test Category 2").Build()
        };

        _fixture.DbContext.Categories.AddRange(categories);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _fixture.Repository.GetAllByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Contains(name, c.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAllByNameAsync_ShouldOnlyReturnActiveCategories()
    {
        // Arrange
        var name = "Category";
        var categories = new List<CategoryEntity>
        {
            new CategoryEntityBuilder().WithName("Category").WithStatus(EntityStatus.Active).Build(),
            new CategoryEntityBuilder().WithName("Inactive Category").WithStatus(EntityStatus.Archived).Build()
        };

        _fixture.DbContext.Categories.AddRange(categories);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _fixture.Repository.GetAllByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, c => Assert.Equal(EntityStatus.Active, c.Status));
        Assert.DoesNotContain(result, c => c.Name == "Inactive Category");
    }
}
