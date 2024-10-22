using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Create;

public class CreateIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public CreateIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var request = new CreateCategoryRequest { Category = createCategory };

        // Act
        var response = await _fixture.Client.CreateCategoryAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Category);
        Assert.Equal(createCategory.Name, response.Category.Name);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldPersist()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var request = new CreateCategoryRequest { Category = createCategory };

        // Act
        var response = await _fixture.Client.CreateCategoryAsync(request);
        var category = await _fixture.Client.GetCategoryAsync(new GetCategoryRequest { Key = response.Category.Key });

        // Assert
        Assert.NotNull(category);
        Assert.Equal(createCategory.Name, category.Category.Name);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsErrorResponse()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("").Build();
        var request = new CreateCategoryRequest { Category = createCategory };

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.CreateCategoryAsync(request));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }
}
