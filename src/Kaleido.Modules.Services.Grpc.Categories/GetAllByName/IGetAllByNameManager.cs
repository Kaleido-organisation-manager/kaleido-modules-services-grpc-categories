using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public interface IGetAllByNameManager
{
    Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default);
}
