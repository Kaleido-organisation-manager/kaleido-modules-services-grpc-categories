using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public class GetAllRevisionsManager : IGetAllRevisionsManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _lifeCycleHandler;
    private readonly ILogger<GetAllRevisionsManager> _logger;

    public GetAllRevisionsManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> lifeCycleHandler,
        ILogger<GetAllRevisionsManager> logger
        )
    {
        _lifeCycleHandler = lifeCycleHandler;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> HandleAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("getting all revisions for category with key: {Key}", key);
        var categoryKey = Guid.Parse(key);
        return await _lifeCycleHandler.GetAllAsync(categoryKey, cancellationToken);
    }
}
