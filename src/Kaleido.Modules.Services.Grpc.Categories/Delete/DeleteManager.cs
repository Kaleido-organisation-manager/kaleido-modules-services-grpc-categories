using AutoMapper;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteManager : IDeleteManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _categoryLifeCycleHandler;
    private readonly ILogger<DeleteManager> _logger;
    private readonly IMapper _mapper;

    public DeleteManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> categoryLifeCycleHandler,
        ILogger<DeleteManager> logger,
        IMapper mapper
        )
    {
        _categoryLifeCycleHandler = categoryLifeCycleHandler;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> DeleteCategoryAsync(string key, CancellationToken cancellationToken = default)
    {
        var categoryKey = Guid.Parse(key);
        _logger.LogInformation("Deleting category with key: {CategoryKey}", categoryKey);
        try
        {
            return await _categoryLifeCycleHandler.DeleteAsync(categoryKey, cancellationToken);
        }
        catch (RevisionNotFoundException)
        {
            return null;
        }
    }
}