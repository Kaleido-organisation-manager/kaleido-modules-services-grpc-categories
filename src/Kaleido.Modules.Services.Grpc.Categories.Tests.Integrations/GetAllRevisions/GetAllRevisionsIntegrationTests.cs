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
        var createCategory = new CategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new CategoryRequest { Key = createResponse.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Categories);
        Assert.NotEmpty(getAllRevisionsResponse.Categories);
        Assert.Single(getAllRevisionsResponse.Categories);
        Assert.Equal("Created", getAllRevisionsResponse.Categories[0].Revision.Action);
        Assert.Equal(1, getAllRevisionsResponse.Categories[0].Revision.Revision);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisions_WhenCategoryIsDeleted()
    {
        // Arrange
        var createCategory = new CategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        await _fixture.Client.DeleteCategoryAsync(new CategoryRequest { Key = createResponse.Key });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new CategoryRequest { Key = createResponse.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Categories);
        Assert.NotEmpty(getAllRevisionsResponse.Categories);
        Assert.Equal(2, getAllRevisionsResponse.Categories.Count);
        Assert.Equal("Deleted", getAllRevisionsResponse.Categories.OrderByDescending(r => r.Revision.Revision).First().Revision.Action);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisions_WhenCategoryIsUpdated()
    {
        // Arrange
        var createCategory = new CategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        var updatedCategory = new CategoryBuilder()
            .WithName("Updated Name")
            .Build();

        await _fixture.Client.UpdateCategoryAsync(new CategoryActionRequest { Key = createResponse.Key, Category = updatedCategory });

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new CategoryRequest { Key = createResponse.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Categories);
        Assert.NotEmpty(getAllRevisionsResponse.Categories);
        Assert.Equal(2, getAllRevisionsResponse.Categories.Count);
        Assert.Equal("Created", getAllRevisionsResponse.Categories.FirstOrDefault(r => r.Revision.Revision == 1)?.Revision.Action);
        Assert.Equal("Updated", getAllRevisionsResponse.Categories.FirstOrDefault(r => r.Revision.Revision == 2)?.Revision.Action);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnRevisionsWithCorrectKey_ForMultipleCategories()
    {
        // Arrange
        var createCategory1 = new CategoryBuilder().Build();
        var createResponse1 = await _fixture.Client.CreateCategoryAsync(createCategory1);

        var createCategory2 = new CategoryBuilder().Build();
        var createResponse2 = await _fixture.Client.CreateCategoryAsync(createCategory2);

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new CategoryRequest { Key = createResponse1.Key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Categories);
        Assert.NotEmpty(getAllRevisionsResponse.Categories);
        Assert.Single(getAllRevisionsResponse.Categories);
        Assert.Equal(createResponse1.Key, getAllRevisionsResponse.Categories[0].Key);
    }

    [Fact]
    public async Task GetAllRevisions_ShouldReturnEmptyList_WhenCategoryDoesNotExist()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();

        // Act
        var getAllRevisionsResponse = await _fixture.Client.GetAllCategoryRevisionsAsync(new CategoryRequest { Key = key });

        // Assert
        Assert.NotNull(getAllRevisionsResponse.Categories);
        Assert.Empty(getAllRevisionsResponse.Categories);
    }
}
