using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public interface IGetAllManager
{
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
