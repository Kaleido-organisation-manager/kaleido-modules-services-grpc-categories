using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;
using Renci.SshNet.Security;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Delete;

[Collection("Infrastructure collection")]
public class DeleteIntegrationTests
{
    private readonly InfrastructureFixture _fixture;

    public DeleteIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task Delete_WithValidRequest_ShouldDelete()
    {
        // Arrange
        var createCategory = new CategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        // Act
        var deleteResponse = await _fixture.Client.DeleteCategoryAsync(new CategoryRequest { Key = createResponse.Key });

        // Assert
        Assert.Equal(createResponse.Key, deleteResponse.Key);
    }

    [Fact]
    public async Task Delete_WithNonExistentKey_ReturnsNotFound()
    {
        // Arrange
        var deleteRequest = new CategoryRequest { Key = Guid.NewGuid().ToString() };

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.DeleteCategoryAsync(deleteRequest));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task Delete_WithValidRequest_ShouldSoftDelete()
    {
        // Arrange
        var createCategory = new CategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        // Act
        var deleteResponse = await _fixture.Client.DeleteCategoryAsync(new CategoryRequest { Key = createResponse.Key });
        var getRevisionResponse = await _fixture.Client.GetCategoryRevisionAsync(
            new GetCategoryRevisionRequest { Key = createResponse.Key, CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow) }
        );

        // Assert
        Assert.Equal(createResponse.Key, deleteResponse.Key);
        Assert.Equal(createResponse.Key, getRevisionResponse.Key);
        Assert.Equal("Deleted", getRevisionResponse.Revision.Action);
    }
}
