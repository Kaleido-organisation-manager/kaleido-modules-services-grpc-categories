using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public interface IGetRevisionManager
{
    Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> GetRevisionAsync(string key, DateTime createdAt, CancellationToken cancellationToken = default);
}
