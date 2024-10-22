using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public interface ICreateManager
{
    Task<Category> CreateAsync(CreateCategory createCategory, CancellationToken cancellationToken = default);
}
