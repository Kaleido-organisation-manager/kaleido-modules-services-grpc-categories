using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAllRevisions;

public class GetAllRevisionsIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public GetAllRevisionsIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisions_WhenCategoryExists()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Revisions);
        Assert.NotEmpty(getAllRevisionsResponse.Revisions);
        Assert.Single(getAllRevisionsResponse.Revisions);
        Assert.Equal("Active", getAllRevisionsResponse.Revisions[0].Status);
        Assert.Equal(1, getAllRevisionsResponse.Revisions[0].Revision);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisions_WhenCategoryIsDeleted()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        await _fixture.Client.DeleteCategoryAsync(new DeleteCategoryRequest { Key = createResponse.Category.Key });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Revisions);
        Assert.NotEmpty(getAllRevisionsResponse.Revisions);
        Assert.Single(getAllRevisionsResponse.Revisions);
        Assert.Equal("Deleted", getAllRevisionsResponse.Revisions.OrderByDescending(r => r.Revision).First().Status);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisions_WhenCategoryIsUpdated()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        var updatedCategory = new CategoryBuilder()
            .WithKey(createResponse.Category.Key)
            .WithName("Updated Name")
            .Build();

        await _fixture.Client.UpdateCategoryAsync(new UpdateCategoryRequest { Key = createResponse.Category.Key, Category = updatedCategory });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Revisions);
        Assert.NotEmpty(getAllRevisionsResponse.Revisions);
        Assert.Equal(2, getAllRevisionsResponse.Revisions.Count);
        Assert.Equal("Archived", getAllRevisionsResponse.Revisions.FirstOrDefault(r => r.Revision == 1)?.Status);
        Assert.Equal("Active", getAllRevisionsResponse.Revisions.FirstOrDefault(r => r.Revision == 2)?.Status);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisionsWithCorrectKey_ForMultipleCategories()
    {
        // Arrange
        var createCategory1 = new CreateCategoryBuilder().Build();
        var createResponse1 = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory1 });

        var createCategory2 = new CreateCategoryBuilder().Build();
        var createResponse2 = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory2 });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest { Key = createResponse1.Category.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Revisions);
        Assert.NotEmpty(getAllRevisionsResponse.Revisions);
        Assert.Single(getAllRevisionsResponse.Revisions);
        Assert.Equal(createResponse1.Category.Key, getAllRevisionsResponse.Revisions[0].Key);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnEmptyList_WhenCategoryDoesNotExist()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new GetAllCategoryRevisionsRequest { Key = key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Revisions);
        Assert.Empty(getAllRevisionsResponse.Revisions);
    }
}
