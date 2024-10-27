using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameManager : IGetAllByNameManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity> _lifeCycleHandler;
    private readonly ILogger<GetAllByNameManager> _logger;

    public GetAllByNameManager(
        IEntityLifecycleHandler<CategoryEntity> repository,
        ILogger<GetAllByNameManager> logger
        )
    {
        _lifeCycleHandler = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all categories by name: {name}", name);
        return await _lifeCycleHandler.FindAllAsync((x) => x.Name == name, cancellationToken: cancellationToken);
    }
}
