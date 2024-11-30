using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetRevision;

[Collection(nameof(InfrastructureCollection))]
public class GetRevisionIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public GetRevisionIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task GetRevision_NonExistentCategory_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentKey = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.GetRevisionAsync(nonExistentKey, DateTime.UtcNow));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetRevision_CreatedCategory_ReturnsCreatedRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");
        var createdAt = created.Revision.CreatedAt;

        // Act
        var revision = await _fixture.Client.GetRevisionAsync(created.Key, createdAt);

        // Assert
        Assert.NotNull(revision);
        Assert.Equal(created.Key, revision.Key);
        Assert.Equal("Test Category", revision.Category.Name);
        Assert.Equal(RevisionAction.Created, revision.Revision.Action);
        Assert.Equal(RevisionStatus.Active, revision.Revision.Status);
        Assert.Equal(1, revision.Revision.Revision);
    }

    [Fact]
    public async Task GetRevision_UpdatedCategory_ReturnsCorrectRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var updated = await _fixture.Client.UpdateAsync(created.Key, "Updated Name");

        // Act
        var originalRevision = await _fixture.Client.GetRevisionAsync(created.Key, created.Revision.CreatedAt);
        var updatedRevision = await _fixture.Client.GetRevisionAsync(updated.Key, updated.Revision.CreatedAt);

        // Assert
        Assert.Equal("Original Name", originalRevision.Category.Name);
        Assert.Equal(RevisionAction.Created, originalRevision.Revision.Action);
        Assert.Equal("Updated Name", updatedRevision.Category.Name);
        Assert.Equal(RevisionAction.Updated, updatedRevision.Revision.Action);
    }

    [Fact]
    public async Task GetRevision_DeletedCategory_ReturnsDeletedRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");
        var deleted = await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var revision = await _fixture.Client.GetRevisionAsync(deleted.Key, deleted.Revision.CreatedAt);

        // Assert
        Assert.NotNull(revision);
        Assert.Equal(deleted.Key, revision.Key);
        Assert.Equal(RevisionAction.Deleted, revision.Revision.Action);
        Assert.Equal(RevisionStatus.Active, revision.Revision.Status);
    }

    [Fact]
    public async Task GetRevision_WithInvalidTimestamp_ThrowsNotFoundException()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");
        var invalidTimestamp = DateTime.UtcNow.AddDays(-1); // Past timestamp

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.GetRevisionAsync(created.Key, invalidTimestamp));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetRevision_MultipleUpdates_ReturnsCorrectRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        var firstUpdate = await _fixture.Client.UpdateAsync(created.Key, "First Update");
        var secondUpdate = await _fixture.Client.UpdateAsync(created.Key, "Second Update");

        // Act
        var originalRevision = await _fixture.Client.GetRevisionAsync(created.Key, created.Revision.CreatedAt);
        var firstUpdateRevision = await _fixture.Client.GetRevisionAsync(firstUpdate.Key, firstUpdate.Revision.CreatedAt);
        var secondUpdateRevision = await _fixture.Client.GetRevisionAsync(secondUpdate.Key, secondUpdate.Revision.CreatedAt);

        // Assert
        Assert.Equal("Original Name", originalRevision.Category.Name);
        Assert.Equal(1, originalRevision.Revision.Revision);
        Assert.Equal("First Update", firstUpdateRevision.Category.Name);
        Assert.Equal(2, firstUpdateRevision.Revision.Revision);
        Assert.Equal("Second Update", secondUpdateRevision.Category.Name);
        Assert.Equal(3, secondUpdateRevision.Revision.Revision);
    }

    [Fact]
    public async Task GetRevision_ReturnsCorrectMetadata()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");

        // Act
        var revision = await _fixture.Client.GetRevisionAsync(created.Key, created.Revision.CreatedAt);

        // Assert
        Assert.NotNull(revision);
        Assert.NotEqual(Guid.Empty, revision.Revision.Key);
        Assert.Equal(created.Key, revision.Key);
        Assert.Equal(RevisionStatus.Active, revision.Revision.Status);
        Assert.Equal(RevisionAction.Created, revision.Revision.Action);
    }
}

