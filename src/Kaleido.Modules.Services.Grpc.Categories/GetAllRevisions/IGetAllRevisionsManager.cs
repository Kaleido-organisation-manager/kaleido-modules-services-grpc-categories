using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public interface IGetAllRevisionsManager
{
    Task<IEnumerable<CategoryRevision>> HandleAsync(string key, CancellationToken cancellationToken = default);
}
