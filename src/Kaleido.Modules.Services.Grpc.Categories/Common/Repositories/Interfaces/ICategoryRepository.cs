using Kaleido.Common.Services.Grpc.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

public interface ICategoryRepository : IBaseRepository<CategoryEntity>
{
    Task<IEnumerable<CategoryEntity>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default);
}