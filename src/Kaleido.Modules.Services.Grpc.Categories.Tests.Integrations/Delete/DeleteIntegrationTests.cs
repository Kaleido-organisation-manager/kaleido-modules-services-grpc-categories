using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Delete;

public class DeleteIntegrationTests : IClassFixture<InfrastructureFixture>
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
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        // Act
        var deleteResponse = await _fixture.Client.DeleteCategoryAsync(new DeleteCategoryRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.Equal(createResponse.Category.Key, deleteResponse.Key);
    }

    [Fact]
    public async Task Delete_WithNonExistentKey_ReturnsNotFound()
    {
        // Arrange
        var deleteRequest = new DeleteCategoryRequest { Key = Guid.NewGuid().ToString() };

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.DeleteCategoryAsync(deleteRequest));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task Delete_WithValidRequest_ShouldSoftDelete()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        // Act
        var deleteResponse = await _fixture.Client.DeleteCategoryAsync(new DeleteCategoryRequest { Key = createResponse.Category.Key });
        var getRevisionResponse = await _fixture.Client.GetCategoryRevisionAsync(new GetCategoryRevisionRequest { Key = createResponse.Category.Key, Revision = 1 });

        // Assert
        Assert.Equal(createResponse.Category.Key, deleteResponse.Key);
        Assert.Equal(createResponse.Category.Key, getRevisionResponse.Revision.Key);
        Assert.Equal("Deleted", getRevisionResponse.Revision.Status);
    }
}
