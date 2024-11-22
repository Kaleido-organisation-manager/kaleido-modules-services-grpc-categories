using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllFilteredManager : IGetAllFilteredManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _lifeCycleHandler;
    private readonly ILogger<GetAllFilteredManager> _logger;

    public GetAllFilteredManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> repository,
        ILogger<GetAllFilteredManager> logger
        )
    {
        _lifeCycleHandler = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all categories by name: {name}", name);
        var matchingCategories = await _lifeCycleHandler.FindAllAsync((x) => x.Name.ToLower().Contains(name.ToLower()), cancellationToken: cancellationToken);
        return matchingCategories.Where(c => c.Revision.Action != RevisionAction.Deleted && c.Revision.Status == RevisionStatus.Active);
    }
}
