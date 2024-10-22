using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public class UpdateManager : IUpdateManager
{

    private readonly ICategoryRepository _repository;
    private readonly ILogger<UpdateManager> _logger;
    private readonly ICategoryMapper _mapper;

    public UpdateManager(
        ICategoryRepository repository,
        ILogger<UpdateManager> logger,
        ICategoryMapper mapper
    )
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Category?> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with key: {Key}", category.Key);

        var categoryKey = Guid.Parse(category.Key);
        var storedCategory = await _repository.GetActiveAsync(categoryKey, cancellationToken);

        if (storedCategory == null)
        {
            return null;
        }

        var newRevision = storedCategory.Revision + 1;
        var categoryEntity = _mapper.ToEntity(category, newRevision);

        if (storedCategory.Equals(categoryEntity))
        {
            _logger.LogWarning("Category with key: {Key} has not changed", category.Key);
            return category;
        }

        var updatedCategory = await _repository.UpdateAsync(categoryEntity, cancellationToken);

        return _mapper.ToCategory(updatedCategory);
    }
}
