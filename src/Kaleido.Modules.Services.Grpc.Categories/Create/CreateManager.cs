using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public class CreateManager : ICreateManager
{
    private readonly ICategoryMapper _categoryMapper;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateManager> _logger;

    public CreateManager(
        ICategoryMapper categoryMapper,
        ICategoryRepository categoryRepository,
        ILogger<CreateManager> logger)
    {
        _categoryMapper = categoryMapper;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Category> CreateAsync(CreateCategory createCategory, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category with name: {Name}", createCategory.Name);

        var category = _categoryMapper.ToCreateEntity(createCategory);

        await _categoryRepository.CreateAsync(category, cancellationToken);

        return _categoryMapper.ToCategory(category);
    }
}