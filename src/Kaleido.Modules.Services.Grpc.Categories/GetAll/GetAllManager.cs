using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllManager : IGetAllManager
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<GetAllManager> _logger;
    private readonly ICategoryMapper _mapper;

    public GetAllManager(ICategoryRepository repository, ILogger<GetAllManager> logger, ICategoryMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all active categories");
        var categories = await _repository.GetAllActiveAsync(cancellationToken);

        return categories.Select(_mapper.ToCategory).ToList();
    }
}