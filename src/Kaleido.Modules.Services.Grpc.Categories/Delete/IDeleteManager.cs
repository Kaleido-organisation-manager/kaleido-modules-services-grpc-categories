using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public interface IDeleteManager
{
    Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> DeleteCategoryAsync(string key, CancellationToken cancellationToken = default);
}
