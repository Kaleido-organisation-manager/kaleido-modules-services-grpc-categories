using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Exists;

public class ExistsIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public ExistsIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Exists_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var createCategory = new CreateCategoryBuilder().WithName("Test Category").Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(new CreateCategoryRequest { Category = createCategory });

        // Act
        var existsResponse = await _fixture.Client.CategoryExistsAsync(new CategoryExistsRequest { Key = createResponse.Category.Key });

        // Assert
        Assert.True(existsResponse.Exists);
    }

    [Fact]
    public async Task Exists_WithNonExistentKey_ShouldReturnFalse()
    {
        // Act
        var existsResponse = await _fixture.Client.CategoryExistsAsync(new CategoryExistsRequest { Key = Guid.NewGuid().ToString() });

        // Assert
        Assert.False(existsResponse.Exists);
    }
}
