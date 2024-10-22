using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Update;

public class UpdateIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public UpdateIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task Update_ShouldUpdateCategory()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Original Name").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        var updatedCategory = new Category
        {
            Key = createResponse.Category.Key,
            Name = "Updated Name"
        };

        // Act
        var updateResponse = await _fixture.Client.UpdateCategoryAsync(new UpdateCategoryRequest { Key = createResponse.Category.Key, Category = updatedCategory });

        // Assert
        Assert.NotNull(updateResponse);
        Assert.Equal(createResponse.Category.Key, updateResponse.Category.Key);
        Assert.Equal("Updated Name", updateResponse.Category.Name);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryKey = Guid.NewGuid().ToString();
        var updatedCategory = new Category
        {
            Key = categoryKey,
            Name = "Updated Name"
        };
        var request = new UpdateCategoryRequest { Key = categoryKey, Category = updatedCategory };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.UpdateCategoryAsync(request));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldIncrementRevision()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Original Name").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        var updatedCategory = new Category
        {
            Key = createResponse.Category.Key,
            Name = "Updated Name"
        };

        // Act
        var updateResponse = await _fixture.Client.UpdateCategoryAsync(new UpdateCategoryRequest { Key = createResponse.Category.Key, Category = updatedCategory });
        var categoryRevisions = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest() { Key = createResponse.Category.Key });

        // Assert
        Assert.NotNull(updateResponse);
        Assert.Equal(2, categoryRevisions.Revisions.Count);
        Assert.Equal(2, categoryRevisions.Revisions.First(c => c.Name == "Updated Name").Revision);
    }
}

