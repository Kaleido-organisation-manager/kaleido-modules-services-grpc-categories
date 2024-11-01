using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.GetRevision;

public class GetRevisionIntegrationTests : IClassFixture<InfrastructureFixture>
{
    private readonly InfrastructureFixture _fixture;

    public GetRevisionIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearDatabase().Wait();
    }

    [Fact]
    public async Task GetCategoryRevision_ShouldReturnCategory_WhenCategoryExists()
    {
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        var request = new GetCategoryRevisionRequest { Key = createResponse.Key, Revision = 1 };
        var response = await _fixture.Client.GetCategoryRevisionAsync(request);

        Assert.NotNull(response.Revision);
        Assert.Equal(createResponse.Key, response.Revision.Key);
        Assert.Equal(1, response.Revision.Revision);
        Assert.Equal("Created", response.Revision.Action);
    }

    [Fact]
    public async Task GetCategoryRevision_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        var request = new GetCategoryRevisionRequest { Key = Guid.NewGuid().ToString(), Revision = 1 };

        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.GetCategoryRevisionAsync(request));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetCategoryRevision_ShouldReturnNotFound_WhenRevisionDoesNotExist()
    {
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        var request = new GetCategoryRevisionRequest { Key = createResponse.Key, Revision = 2 };
        var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.Client.GetCategoryRevisionAsync(request));
        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
    }

    [Fact]
    public async Task GetCategoryRevision_ShouldReturnArchived_WhenCategoryHasBeenArchived()
    {
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        var updatedCategory = new CategoryBuilder()
            .WithName(createResponse.Category.Name + " - Updated")
            .Build();

        await _fixture.Client.UpdateCategoryAsync(new CategoryActionRequest { Key = createResponse.Key, Category = updatedCategory });

        var request = new GetCategoryRevisionRequest { Key = createResponse.Key, Revision = 1 };
        var response = await _fixture.Client.GetCategoryRevisionAsync(request);

        Assert.NotNull(response.Revision);
        Assert.Equal("Created", response.Revision.Action);
        Assert.Equal(1, response.Revision.Revision);
    }

    [Fact]
    public async Task GetCategoryRevision_ShouldReturnDeleted_WhenCategoryHasBeenDeleted()
    {
        var createCategory = new CreateCategoryBuilder().Build();
        var createResponse = await _fixture.Client.CreateCategoryAsync(createCategory);

        var request = new CategoryRequest { Key = createResponse.Key };
        await _fixture.Client.DeleteCategoryAsync(request);

        var getRequest = new GetCategoryRevisionRequest { Key = createResponse.Key, Revision = 2 };
        var response = await _fixture.Client.GetCategoryRevisionAsync(getRequest);

        Assert.NotNull(response.Revision);
        Assert.Equal("Deleted", response.Revision.Action);
        Assert.Equal(2, response.Revision.Revision);
    }
}

