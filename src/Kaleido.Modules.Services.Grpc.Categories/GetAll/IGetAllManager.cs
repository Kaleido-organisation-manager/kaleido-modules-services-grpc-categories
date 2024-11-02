using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public interface IGetAllManager
{
    Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
}
