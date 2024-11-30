using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAll;

[Collection(nameof(InfrastructureCollection))]
public class GetAllIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public GetAllIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task GetAll_WithNoCategories_ReturnsEmptyList()
    {
        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_WithMultipleCategories_ReturnsAllActiveCategories()
    {
        // Arrange
        var category1 = await _fixture.Client.CreateAsync("Test Category 1");
        var category2 = await _fixture.Client.CreateAsync("Test Category 2");
        var category3 = await _fixture.Client.CreateAsync("Test Category 3");

        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.Key == category1.Key);
        Assert.Contains(result, c => c.Key == category2.Key);
        Assert.Contains(result, c => c.Key == category3.Key);
    }

    [Fact]
    public async Task GetAll_WithDeletedCategories_ReturnsOnlyActiveCategories()
    {
        // Arrange
        var category1 = await _fixture.Client.CreateAsync("Test Category 1");
        var category2 = await _fixture.Client.CreateAsync("Test Category 2");
        await _fixture.Client.DeleteAsync(category1.Key);

        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, c => c.Key == category2.Key);
        Assert.DoesNotContain(result, c => c.Key == category1.Key);
    }

    [Fact]
    public async Task GetAll_WithUpdatedCategories_ReturnsLatestVersions()
    {
        // Arrange
        var category1 = await _fixture.Client.CreateAsync("Original Name 1");
        var category2 = await _fixture.Client.CreateAsync("Original Name 2");
        await _fixture.Client.UpdateAsync(category1.Key, "Updated Name 1");

        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Category.Name == "Updated Name 1");
        Assert.Contains(result, c => c.Category.Name == "Original Name 2");
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectRevisionInformation()
    {
        // Arrange
        var category = await _fixture.Client.CreateAsync("Test Category");
        await _fixture.Client.UpdateAsync(category.Key, "Updated Category");

        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        var updatedCategory = result.Single(c => c.Key == category.Key);
        Assert.Equal(RevisionAction.Updated, updatedCategory.Revision.Action);
        Assert.Equal(RevisionStatus.Active, updatedCategory.Revision.Status);
    }

    [Fact]
    public async Task GetAll_WithManyCategories_ReturnsAllCategories()
    {
        // Arrange
        var expectedCount = 10;
        for (int i = 0; i < expectedCount; i++)
        {
            await _fixture.Client.CreateAsync($"Test Category {i}");
        }

        // Act
        var result = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count());
    }
}
