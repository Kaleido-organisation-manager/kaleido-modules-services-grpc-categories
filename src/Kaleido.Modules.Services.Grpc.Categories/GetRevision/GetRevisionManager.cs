using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionManager : IGetRevisionManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity> _lifeCycleHandler;
    private readonly ILogger<GetRevisionManager> _logger;

    public GetRevisionManager(
        IEntityLifecycleHandler<CategoryEntity> lifecycleHandler,
        ILogger<GetRevisionManager> logger
    )
    {
        _lifeCycleHandler = lifecycleHandler;
        _logger = logger;
    }

    public async Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> GetRevisionAsync(string key, int revision, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting revision {Revision} for category with key: {Key}", revision, key);

        var categoryKey = Guid.Parse(key);
        return await _lifeCycleHandler.GetAsync(categoryKey, revision, cancellationToken);


    }
}
