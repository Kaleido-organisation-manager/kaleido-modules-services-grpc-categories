using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public class GetManager : IGetManager
{
    private readonly ICategoryMapper _mapper;
    private readonly ICategoryRepository _repository;
    private readonly ILogger<GetManager> _logger;

    public GetManager(
        ICategoryMapper mapper,
        ICategoryRepository repository,
        ILogger<GetManager> logger
    )
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Category?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting category with key: {Key}", key);
        var categoryEntity = await _repository.GetActiveAsync(Guid.Parse(key), cancellationToken);
        if (categoryEntity == null)
        {
            return null;
        }
        return _mapper.ToCategory(categoryEntity);
    }
}
