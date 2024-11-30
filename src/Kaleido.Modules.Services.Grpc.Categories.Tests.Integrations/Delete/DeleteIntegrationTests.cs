using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Delete;

[Collection(nameof(InfrastructureCollection))]
public class DeleteIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public DeleteIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task DeleteCategory_ValidKey_DeletesCategory()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);

        // Act
        var deleted = await _fixture.Client.DeleteAsync(created.Key);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(created.Key, deleted.Key);
        Assert.Equal(name, deleted.Category.Name);
        Assert.NotNull(deleted.Revision);
        Assert.Equal(RevisionAction.Deleted, deleted.Revision.Action);
        Assert.Equal(RevisionStatus.Active, deleted.Revision.Status);
    }

    [Fact]
    public async Task DeleteCategory_NonExistentKey_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentKey = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.DeleteAsync(nonExistentKey));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_DeletedCategory_CanBeCreatedAgain()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var newCategory = await _fixture.Client.CreateAsync(name);

        // Assert
        Assert.NotNull(newCategory);
        Assert.NotEqual(created.Key, newCategory.Key); // Should be a new category
        Assert.Equal(name, newCategory.Category.Name);
        Assert.NotNull(newCategory.Revision);
        Assert.Equal(RevisionAction.Created, newCategory.Revision.Action);
        Assert.Equal(RevisionStatus.Active, newCategory.Revision.Status);
    }

    [Fact]
    public async Task DeleteCategory_DeletedCategory_CannotBeDeletedAgain()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.DeleteAsync(created.Key));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_DeletedCategory_NotReturnedInGetAll()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var allCategories = await _fixture.Client.GetAllAsync();

        // Assert
        Assert.DoesNotContain(allCategories, c => c.Key == created.Key);
    }

    [Fact]
    public async Task DeleteCategory_DeletedCategory_StillVisibleInRevisions()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        Assert.NotEmpty(revisions);
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Created);
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Deleted);
    }

    [Fact]
    public async Task DeleteCategory_DeletedCategory_CannotBeRetrieved()
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
}
