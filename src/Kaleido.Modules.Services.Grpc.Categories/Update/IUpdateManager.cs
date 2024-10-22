using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public interface IUpdateManager
{
    Task<Category?> UpdateAsync(Category request, CancellationToken cancellationToken = default);
}
