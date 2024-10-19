using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameManager : IGetAllByNameManager
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<GetAllByNameManager> _logger;
    private readonly ICategoryMapper _mapper;

    public GetAllByNameManager(
        ICategoryRepository repository,
        ILogger<GetAllByNameManager> logger,
        ICategoryMapper mapper
        )
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all categories by name: {name}", name);
        var categories = await _repository.GetAllByNameAsync(name, cancellationToken);

        return categories.Select(_mapper.ToCategory).ToList();
    }
}
