using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteManager : IDeleteManager
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<DeleteManager> _logger;

    public DeleteManager(
        ICategoryRepository categoryRepository,
        ILogger<DeleteManager> logger
        )
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<CategoryEntity?> DeleteCategoryAsync(string key, CancellationToken cancellationToken = default)
    {
        var categoryKey = Guid.Parse(key);
        _logger.LogInformation("Deleting category with key: {CategoryKey}", categoryKey);
        var deletedEntity = await _categoryRepository.DeleteAsync(categoryKey, cancellationToken);

        return deletedEntity;
    }
}