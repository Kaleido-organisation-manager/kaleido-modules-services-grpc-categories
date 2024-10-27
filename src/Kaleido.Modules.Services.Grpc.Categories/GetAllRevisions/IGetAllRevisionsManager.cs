using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public interface IGetAllRevisionsManager
{
    Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> HandleAsync(string key, CancellationToken cancellationToken = default);
}
