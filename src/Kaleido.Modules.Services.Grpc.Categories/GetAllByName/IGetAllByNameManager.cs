using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public interface IGetAllByNameManager
{
    Task<IEnumerable<Category>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default);
}
