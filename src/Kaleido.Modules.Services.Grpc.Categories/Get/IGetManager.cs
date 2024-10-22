using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public interface IGetManager
{
    Task<Category?> GetAsync(string key, CancellationToken cancellationToken = default);
}
