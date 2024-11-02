using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public class UpdateManager : IUpdateManager
{

    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _lifeCycleHandler;
    private readonly ILogger<UpdateManager> _logger;

    public UpdateManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> lifecycleHandler,
        ILogger<UpdateManager> logger
    )
    {
        _lifeCycleHandler = lifecycleHandler;
        _logger = logger;
    }

    public async Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> UpdateAsync(Guid key, CategoryEntity category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with key: {Key}", key);

        try
        {
            return await _lifeCycleHandler.UpdateAsync(key, category, cancellationToken);
        }
        catch (RevisionNotFoundException)
        {
            return null;
        }
    }
}
