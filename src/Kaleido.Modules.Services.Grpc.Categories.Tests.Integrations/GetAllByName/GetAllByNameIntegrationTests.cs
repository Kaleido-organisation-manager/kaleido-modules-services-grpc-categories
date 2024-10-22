using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetAllByName;

public class GetAllByNameIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public GetAllByNameIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task GetAllByName_WithNoCategories_ShouldReturnEmptyList()
    {
        // Act
        var getAllByNameResponse = await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "Test" });

        // Assert
        Assert.Empty(getAllByNameResponse.Categories);
    }

    [Fact]
    public async Task GetAllByName_WithCategories_ShouldReturnCategories()
    {
        // Arrange
        var category = new CreateCategoryBuilder().WithName("Test").Build();
        await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = category });

        // Act
        var getAllByNameResponse = await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "Test" });

        // Assert
        Assert.Single(getAllByNameResponse.Categories);
    }

    [Fact]
    public async Task GetAllByName_WithCategories_ShouldReturnPartialMatches()
    {
        var categories = new[]
        {
            new CreateCategoryBuilder().WithName("Test").Build(),
            new CreateCategoryBuilder().WithName("Test2").Build(),
            new CreateCategoryBuilder().WithName("Test3").Build(),
            new CreateCategoryBuilder().WithName("NonMatch").Build()
        };

        foreach (var category in categories)
        {
            await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = category });
        }

        // Act
        var getAllByNameResponse = await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "Test" });

        // Assert
        Assert.Equal(3, getAllByNameResponse.Categories.Count);
    }

    [Fact]
    public async Task GetAllByName_WithInvalidName_ShouldReturnValidationErrors()
    {
        // Act
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "" }));

        // Assert
        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetAllByName_WithNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        var category = new CreateCategoryBuilder().WithName("Test").Build();
        await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = category });

        // Act
        var getAllByNameResponse = await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "NonMatch" });

        // Assert
        Assert.Empty(getAllByNameResponse.Categories);
    }

    [Fact]
    public async Task GetAllByName_ShouldBeCaseInsensitive()
    {
        // Arrange
        var category = new CreateCategoryBuilder().WithName("Test").Build();
        await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = category });

        // Act
        var getAllByNameResponse = await _fixture.Client.GetAllCategoriesByNameAsync(new GetAllCategoriesByNameRequest { Name = "test" });

        // Assert
        Assert.Single(getAllByNameResponse.Categories);
    }
}
