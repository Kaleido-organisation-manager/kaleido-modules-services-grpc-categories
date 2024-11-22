using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionManager : IGetRevisionManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _lifeCycleHandler;
    private readonly ILogger<GetRevisionManager> _logger;

    public GetRevisionManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> lifecycleHandler,
        ILogger<GetRevisionManager> logger
    )
    {
        _lifeCycleHandler = lifecycleHandler;
        _logger = logger;
    }

    public async Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> GetRevisionAsync(string key, DateTime createdAt, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting revision for category with key: {Key} at {CreatedAt}", key, createdAt);

        var categoryKey = Guid.Parse(key);
        return await _lifeCycleHandler.GetHistoricAsync(categoryKey, createdAt, cancellationToken);


    }
}
