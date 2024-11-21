using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAll;

[Collection("Infrastructure collection")]
public class GetAllIntegrationTests
{
    private readonly InfrastructureFixture _fixture;

    public GetAllIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllCategories()
    {
        // Arrange
        var createCategories = new List<CreateCategoryBuilder>
        {
            new CreateCategoryBuilder().WithName("Test Category 1"),
            new CreateCategoryBuilder().WithName("Test Category 2"),
            new CreateCategoryBuilder().WithName("Test Category 3")
        };

        foreach (var createCategory in createCategories)
        {
            await _fixture.Client.CreateCategoryAsync(createCategory.Build());
        }

        // Act
        var getAllResponse = await _fixture.Client.GetAllCategoriesAsync(new EmptyRequest());

        // Assert
        Assert.Equal(createCategories.Count, getAllResponse.Categories.Count);
    }

    [Fact]
    public async Task GetAll_WithNoCategories_ShouldReturnEmptyList()
    {
        // Act
        var getAllResponse = await _fixture.Client.GetAllCategoriesAsync(new EmptyRequest());

        // Assert
        Assert.Empty(getAllResponse.Categories);
    }
}
