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
        var request = new CategoryBuilder().WithName("Test Category").Build();

        // Act
        var response = await _fixture.Client.CreateCategoryAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Category);
        Assert.Equal(request.Name, response.Category.Name);
        Assert.Equal(1, response.Revision.Revision);
        Assert.Equal(response.Key, response.Key);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldPersist()
    {
        // Arrange
        var request = new CategoryBuilder().WithName("Test Category").Build();

        // Act
        var response = await _fixture.Client.CreateCategoryAsync(request);
        var category = await _fixture.Client.GetCategoryAsync(new CategoryRequest { Key = response.Key });

        // Assert
        Assert.NotNull(category);
        Assert.Equal(request.Name, category.Category.Name);
        Assert.Equal(1, response.Revision.Revision);
        Assert.Equal(response.Key, category.Key);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsErrorResponse()
    {
        // Arrange
        var createCategory = new CategoryBuilder().WithName("").Build();

        // Act && Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.CreateCategoryAsync(createCategory));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }
}
