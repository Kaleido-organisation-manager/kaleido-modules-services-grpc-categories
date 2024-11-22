using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Get;

[Collection("Infrastructure collection")]
public class GetIntegrationTests
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
        var createCategory = new CategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        // Act
        var getResponse = await _fixture.Client.GetCategoryAsync(new CategoryRequest { Key = createResponse.Key });

        // Assert
        Assert.NotNull(getResponse.Category);
        Assert.Equal(createResponse.Key, getResponse.Key);
        Assert.Equal(createResponse.Category.Name, getResponse.Category.Name);
    }

    [Fact]
    public async Task Get_WithNonExistentKey_ShouldReturnNotFound()
    {
        // Act
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.GetCategoryAsync(new CategoryRequest { Key = Guid.NewGuid().ToString() }));

        // Assert
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }
}
