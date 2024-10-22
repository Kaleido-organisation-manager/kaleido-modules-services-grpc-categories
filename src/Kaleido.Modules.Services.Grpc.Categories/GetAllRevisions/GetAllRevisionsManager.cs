using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public class GetAllRevisionsManager : IGetAllRevisionsManager
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<GetAllRevisionsManager> _logger;
    private readonly ICategoryMapper _mapper;

    public GetAllRevisionsManager(
        ICategoryRepository repository,
        ILogger<GetAllRevisionsManager> logger,
        ICategoryMapper mapper
        )
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryRevision>> HandleAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("getting all revisions for category with key: {Key}", key);
        var categoryKey = Guid.Parse(key);
        var revisions = await _repository.GetAllRevisionsAsync(categoryKey, cancellationToken);

        return revisions.Select(_mapper.ToCategoryRevision).ToList();
    }
}
