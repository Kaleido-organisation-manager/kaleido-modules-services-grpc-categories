using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Get;

public class GetIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public GetIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task Get_WithValidRequest_ShouldReturnCategory()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        // Act
        var getResponse = await _fixture.Client.GetCategoryAsync(new GetCategoryRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.NotNull(getResponse.Category);
        Assert.Equal(createResponse.Category.Key, getResponse.Category.Key);
        Assert.Equal(createResponse.Category.Name, getResponse.Category.Name);
    }

    [Fact]
    public async Task Get_WithNonExistentKey_ShouldReturnNotFound()
    {
        // Act
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.GetCategoryAsync(new GetCategoryRequest { Key = Guid.NewGuid().ToString() }));

        // Assert
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }
}
