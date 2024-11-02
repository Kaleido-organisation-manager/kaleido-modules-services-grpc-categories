using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllManager : IGetAllManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> _categoryLifeCycleHandler;
    private readonly ILogger<GetAllManager> _logger;

    public GetAllManager(
        IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity> repository,
         ILogger<GetAllManager> logger
         )
    {
        _categoryLifeCycleHandler = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all active categories");
        var categories = await _categoryLifeCycleHandler.GetAllAsync(cancellationToken: cancellationToken);
        return categories.Where(c => c.Revision.Action != RevisionAction.Deleted);
    }
}