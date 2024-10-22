using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public interface IDeleteManager
{
    Task<CategoryEntity?> DeleteCategoryAsync(string key, CancellationToken cancellationToken = default);
}
