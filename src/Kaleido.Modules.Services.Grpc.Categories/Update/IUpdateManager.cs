using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public interface IUpdateManager
{
    Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> UpdateAsync(Guid key, CategoryEntity category, CancellationToken cancellationToken = default);
}
