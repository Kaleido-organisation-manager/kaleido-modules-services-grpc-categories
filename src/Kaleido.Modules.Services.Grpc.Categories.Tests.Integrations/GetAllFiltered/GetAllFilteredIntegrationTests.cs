using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAllFiltered;

[Collection(nameof(InfrastructureCollection))]
public class GetAllFilteredIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public GetAllFilteredIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task GetAllFiltered_WithNoCategories_ReturnsEmptyList()
    {
        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllFiltered_WithMatchingCategories_ReturnsFilteredList()
    {
        // Arrange
        await _fixture.Client.CreateAsync("Test Category 1");
        await _fixture.Client.CreateAsync("Test Category 2");
        await _fixture.Client.CreateAsync("Different Category");

        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Contains("Test", c.Category.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetAllFiltered_WithInvalidFilter_ThrowsException(string invalidFilter)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.GetAllFilteredAsync(invalidFilter));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetAllFiltered_IsCaseInsensitive()
    {
        // Arrange
        await _fixture.Client.CreateAsync("Test Category");

        // Act
        var result1 = await _fixture.Client.GetAllFilteredAsync("test");
        var result2 = await _fixture.Client.GetAllFilteredAsync("TEST");
        var result3 = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Single(result3);
    }

    [Fact]
    public async Task GetAllFiltered_WithDeletedCategories_ExcludesDeletedCategories()
    {
        // Arrange
        var category1 = await _fixture.Client.CreateAsync("Test Category 1");
        await _fixture.Client.CreateAsync("Test Category 2");
        await _fixture.Client.DeleteAsync(category1.Key);

        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.Single(result);
        Assert.All(result, c => Assert.NotEqual(category1.Key, c.Key));
    }

    [Fact]
    public async Task GetAllFiltered_WithPartialMatch_ReturnsMatchingCategories()
    {
        // Arrange
        await _fixture.Client.CreateAsync("Test Category");
        await _fixture.Client.CreateAsync("Testing Category");
        await _fixture.Client.CreateAsync("Category Test");
        await _fixture.Client.CreateAsync("No Match");

        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllFiltered_WithUpdatedCategories_ReturnsLatestVersions()
    {
        // Arrange
        var category = await _fixture.Client.CreateAsync("Test Original");
        await _fixture.Client.UpdateAsync(category.Key, "Test Updated");

        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        Assert.Single(result);
        Assert.Contains(result, c => c.Category.Name == "Test Updated");
        Assert.DoesNotContain(result, c => c.Category.Name == "Test Original");
    }

    [Fact]
    public async Task GetAllFiltered_ReturnsCorrectRevisionInformation()
    {
        // Arrange
        var category = await _fixture.Client.CreateAsync("Test Category");
        await _fixture.Client.UpdateAsync(category.Key, "Test Updated");

        // Act
        var result = await _fixture.Client.GetAllFilteredAsync("Test");

        // Assert
        var updatedCategory = result.Single();
        Assert.Equal(RevisionAction.Updated, updatedCategory.Revision.Action);
        Assert.Equal(RevisionStatus.Active, updatedCategory.Revision.Status);
    }
}
