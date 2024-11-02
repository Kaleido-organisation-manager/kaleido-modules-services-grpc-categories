using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public interface IGetManager
{
    Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> GetAsync(string key, CancellationToken cancellationToken = default);
}
