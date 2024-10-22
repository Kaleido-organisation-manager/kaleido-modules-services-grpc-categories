using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionManager : IGetRevisionManager
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<GetRevisionManager> _logger;
    private readonly ICategoryMapper _mapper;

    public GetRevisionManager(
        ICategoryRepository repository,
        ILogger<GetRevisionManager> logger,
        ICategoryMapper mapper
    )
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CategoryRevision?> GetRevisionAsync(string key, int revision, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting revision {Revision} for category with key: {Key}", revision, key);

        var categoryKey = Guid.Parse(key);
        var category = await _repository.GetRevisionAsync(categoryKey, revision, cancellationToken);

        if (category == null)
        {
            return null;
        }

        return _mapper.ToCategoryRevision(category);
    }
}
