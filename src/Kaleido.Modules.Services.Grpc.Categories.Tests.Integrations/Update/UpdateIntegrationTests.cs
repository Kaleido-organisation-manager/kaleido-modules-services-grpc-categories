using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Client.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Update;

[Collection(nameof(InfrastructureCollection))]
public class UpdateIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public UpdateIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task UpdateCategory_ValidUpdate_UpdatesCategory()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var newName = "Updated Name";

        // Act
        var result = await _fixture.Client.UpdateAsync(created.Key, newName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Key, result.Key);
        Assert.Equal(newName, result.Category.Name);
        Assert.Equal(RevisionAction.Updated, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Fact]
    public async Task UpdateCategory_NonExistentKey_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentKey = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.UpdateAsync(nonExistentKey, "New Name"));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateCategory_InvalidName_ThrowsException(string invalidName)
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.UpdateAsync(created.Key, invalidName));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_DeletedCategory_ThrowsNotFoundException()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        await _fixture.Client.DeleteAsync(created.Key);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.UpdateAsync(created.Key, "New Name"));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_UsingEntityDto_UpdatesCategory()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var updateDto = new CategoryEntityDto
        {
            Name = "Updated Name"
        };

        // Act
        var result = await _fixture.Client.UpdateAsync(created.Key, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Key, result.Key);
        Assert.Equal(updateDto.Name, result.Category.Name);
        Assert.Equal(RevisionAction.Updated, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Fact]
    public async Task UpdateCategory_WithSameName_CreatesNewRevision()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);

        // Act
        var result = await _fixture.Client.UpdateAsync(created.Key, name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Key, result.Key);
        Assert.Equal(name, result.Category.Name);
        Assert.Equal(RevisionAction.Updated, result.Revision.Action);
        Assert.Equal(created.Revision.Key, result.Revision.Key);
    }

    [Fact]
    public async Task UpdateCategory_MultipleUpdates_CreatesNewRevisions()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");

        // Act
        var firstUpdate = await _fixture.Client.UpdateAsync(created.Key, "First Update");
        var secondUpdate = await _fixture.Client.UpdateAsync(created.Key, "Second Update");

        // Assert
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);
        Assert.Equal(3, revisions.Count());
        Assert.Contains(revisions, r => r.Category.Name == "Original Name" && r.Revision.Action == RevisionAction.Created);
        Assert.Contains(revisions, r => r.Category.Name == "First Update" && r.Revision.Action == RevisionAction.Updated);
        Assert.Contains(revisions, r => r.Category.Name == "Second Update" && r.Revision.Action == RevisionAction.Updated);
    }

    [Fact]
    public async Task UpdateCategory_NameTooLong_ThrowsException()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var longName = new string('a', 101); // Over 100 characters

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.UpdateAsync(created.Key, longName));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_UpdatedCategoryCanBeRetrieved()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var updated = await _fixture.Client.UpdateAsync(created.Key, "Updated Name");

        // Act
        var retrieved = await _fixture.Client.GetAsync(created.Key);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(updated.Key, retrieved.Key);
        Assert.Equal("Updated Name", retrieved.Category.Name);
        Assert.Equal(RevisionAction.Updated, retrieved.Revision.Action);
    }
}

