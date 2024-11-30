using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Get;

[Collection(nameof(InfrastructureCollection))]
public class GetIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public GetIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task GetCategory_ValidKey_ReturnsCategory()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);

        // Act
        var result = await _fixture.Client.GetAsync(created.Key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Key, result.Key);
        Assert.Equal(name, result.Category.Name);
        Assert.NotNull(result.Revision);
        Assert.Equal(RevisionAction.Created, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Fact]
    public async Task GetCategory_NonExistentKey_ThrowsException()
    {
        // Arrange
        var nonExistentKey = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.GetAsync(nonExistentKey));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetCategory_DeletedCategory_ThrowsException()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.GetAsync(created.Key));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetCategory_MultipleCategories_ReturnsCorrectCategory()
    {
        // Arrange
        var name1 = "Test Category 1";
        var name2 = "Test Category 2";

        var created1 = await _fixture.Client.CreateAsync(name1);
        var created2 = await _fixture.Client.CreateAsync(name2);

        // Act
        var result1 = await _fixture.Client.GetAsync(created1.Key);
        var result2 = await _fixture.Client.GetAsync(created2.Key);

        // Assert
        Assert.NotEqual(result1.Key, result2.Key);
        Assert.Equal(name1, result1.Category.Name);
        Assert.Equal(name2, result2.Category.Name);
    }

    [Fact]
    public async Task GetCategory_UpdatedCategory_ReturnsLatestVersion()
    {
        // Arrange
        var originalName = "Original Name";
        var updatedName = "Updated Name";

        var created = await _fixture.Client.CreateAsync(originalName);
        await _fixture.Client.UpdateAsync(created.Key, updatedName);

        // Act
        var result = await _fixture.Client.GetAsync(created.Key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Key, result.Key);
        Assert.Equal(updatedName, result.Category.Name);
        Assert.Equal(RevisionAction.Updated, result.Revision.Action);
    }
}
