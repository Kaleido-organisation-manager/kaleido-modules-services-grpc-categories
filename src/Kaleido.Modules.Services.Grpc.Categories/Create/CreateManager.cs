using AutoMapper;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public class CreateManager : ICreateManager
{
    private readonly IEntityLifecycleHandler<CategoryEntity> _categoryLifeCycleHandler;
    private readonly ILogger<CreateManager> _logger;

    public CreateManager(
        IEntityLifecycleHandler<CategoryEntity> categoryRepository,
        ILogger<CreateManager> logger
        )
    {
        _categoryLifeCycleHandler = categoryRepository;
        _logger = logger;
    }

    public Task<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> CreateAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category with name: {Name}", category.Name);

        return _categoryLifeCycleHandler.CreateAsync(category, cancellationToken);
    }
}
