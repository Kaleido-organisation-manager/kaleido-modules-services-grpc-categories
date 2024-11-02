using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public interface ICreateManager
{
    Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> CreateAsync(CategoryEntity createCategory, CancellationToken cancellationToken = default);
}
