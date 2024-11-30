using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAllRevisions;

[Collection(nameof(InfrastructureCollection))]
public class GetAllRevisionsIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public GetAllRevisionsIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task GetAllRevisions_NonExistentCategory_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentKey = Guid.NewGuid();

        // Act
        var result = await _fixture.Client.GetAllRevisionsAsync(nonExistentKey);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllRevisions_NewlyCreatedCategory_ReturnsSingleRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        Assert.NotNull(revisions);
        Assert.Single(revisions);
        var revision = revisions.First();
        Assert.Equal(RevisionAction.Created, revision.Revision.Action);
        Assert.Equal(RevisionStatus.Active, revision.Revision.Status);
        Assert.Equal(1, revision.Revision.Revision);
    }

    [Fact]
    public async Task GetAllRevisions_UpdatedCategory_ReturnsAllRevisions()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        await _fixture.Client.UpdateAsync(created.Key, "Updated Name");

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        Assert.NotNull(revisions);
        Assert.Equal(2, revisions.Count());
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Created && r.Category.Name == "Original Name");
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Updated && r.Category.Name == "Updated Name");
    }

    [Fact]
    public async Task GetAllRevisions_DeletedCategory_IncludesDeleteRevision()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        Assert.NotNull(revisions);
        Assert.Equal(2, revisions.Count());
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Created);
        Assert.Contains(revisions, r => r.Revision.Action == RevisionAction.Deleted);
    }

    [Fact]
    public async Task GetAllRevisions_MultipleUpdates_ReturnsAllRevisions()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        await _fixture.Client.UpdateAsync(created.Key, "First Update");
        await _fixture.Client.UpdateAsync(created.Key, "Second Update");
        await _fixture.Client.UpdateAsync(created.Key, "Third Update");

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        Assert.NotNull(revisions);
        Assert.Equal(4, revisions.Count());
        Assert.Contains(revisions, r => r.Category.Name == "Original Name" && r.Revision.Action == RevisionAction.Created);
        Assert.Contains(revisions, r => r.Category.Name == "First Update" && r.Revision.Action == RevisionAction.Updated);
        Assert.Contains(revisions, r => r.Category.Name == "Second Update" && r.Revision.Action == RevisionAction.Updated);
        Assert.Contains(revisions, r => r.Category.Name == "Third Update" && r.Revision.Action == RevisionAction.Updated);
    }

    [Fact]
    public async Task GetAllRevisions_RevisionsAreOrderedByRevisionNumber()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Original Name");
        await _fixture.Client.UpdateAsync(created.Key, "Updated Name");
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        var orderedRevisions = revisions.OrderBy(r => r.Revision.Revision).ToList();
        Assert.Equal(1, orderedRevisions[0].Revision.Revision);
        Assert.Equal(2, orderedRevisions[1].Revision.Revision);
        Assert.Equal(3, orderedRevisions[2].Revision.Revision);
    }

    [Fact]
    public async Task GetAllRevisions_RevisionsContainCorrectMetadata()
    {
        // Arrange
        var created = await _fixture.Client.CreateAsync("Test Category");

        // Act
        var revisions = await _fixture.Client.GetAllRevisionsAsync(created.Key);

        // Assert
        var revision = revisions.Single();
        Assert.NotEqual(Guid.Empty, revision.Revision.Key);
        Assert.NotEqual(default, revision.Revision.CreatedAt);
        Assert.Equal(RevisionStatus.Active, revision.Revision.Status);
        Assert.Equal(created.Key, revision.Key);
    }
}
