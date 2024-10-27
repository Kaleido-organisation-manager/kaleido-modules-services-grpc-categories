using AutoMapper;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public class GetManager : IGetManager
{
    private readonly IMapper _mapper;
    private readonly IEntityLifecycleHandler<CategoryEntity> _categoryLifeCycleHandler;
    private readonly ILogger<GetManager> _logger;

    public GetManager(
        IMapper mapper,
        IEntityLifecycleHandler<CategoryEntity> categoryLifeCycleHandler,
        ILogger<GetManager> logger
    )
    {
        _mapper = mapper;
        _categoryLifeCycleHandler = categoryLifeCycleHandler;
        _logger = logger;
    }

    public async Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting category with key: {Key}", key);
        var result = await _categoryLifeCycleHandler.GetAsync(Guid.Parse(key), cancellationToken: cancellationToken);
        if (result == null || result.Revision.Action == RevisionAction.Deleted)
        {
            return null;
        }
        return _mapper.Map<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?>(result);
    }
}
