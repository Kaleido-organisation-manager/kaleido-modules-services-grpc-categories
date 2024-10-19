using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public interface IGetRevisionManager
{
    Task<CategoryRevision?> GetRevisionAsync(string key, int revision, CancellationToken cancellationToken = default);
}
