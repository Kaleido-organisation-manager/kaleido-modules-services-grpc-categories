using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetRevision;

public class GetRevisionRequestValidatorTests
{
    private readonly GetCategoryRevisionRequestValidator _sut;

    public GetRevisionRequestValidatorTests()
    {
        _sut = new GetCategoryRevisionRequestValidator();
    }

    [Fact]
    public async Task ValidateAsync_ValidRequest_ReturnsSuccess()
    {
        var request = new GetCategoryRevisionRequest { Key = Guid.NewGuid().ToString(), Revision = 1 };
        var result = await _sut.ValidateAsync(request);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_InvalidKey_ReturnsError()
    {
        var request = new GetCategoryRevisionRequest { Key = "invalid-key", Revision = 1 };
        var result = await _sut.ValidateAsync(request);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_InvalidRevision_ReturnsError()
    {
        var request = new GetCategoryRevisionRequest { Key = Guid.NewGuid().ToString(), Revision = -1 };
        var result = await _sut.ValidateAsync(request);
        Assert.False(result.IsValid);
    }
}


