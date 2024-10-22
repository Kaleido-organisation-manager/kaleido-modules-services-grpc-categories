using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;
using static Kaleido.Grpc.Categories.GrpcCategories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Services;

public class CategoryService : GrpcCategoriesBase
{
    private readonly ILogger<CategoryService> _logger;
    private readonly IBaseHandler<CreateCategoryRequest, CreateCategoryResponse> _createHandler;
    private readonly IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse> _deleteHandler;
    private readonly IBaseHandler<CategoryExistsRequest, CategoryExistsResponse> _existsHandler;
    private readonly IBaseHandler<GetCategoryRequest, GetCategoryResponse> _getHandler;
    private readonly IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse> _getAllHandler;
    private readonly IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse> _getAllByNameHandler;
    private readonly IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse> _getAllRevisionsHandler;
    private readonly IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse> _getRevisionHandler;
    private readonly IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse> _updateHandler;

    public CategoryService(
        ILogger<CategoryService> logger,
        IBaseHandler<CreateCategoryRequest, CreateCategoryResponse> createHandler,
        IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse> deleteHandler,
        IBaseHandler<CategoryExistsRequest, CategoryExistsResponse> existsHandler,
        IBaseHandler<GetCategoryRequest, GetCategoryResponse> getHandler,
        IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse> getAllHandler,
        IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse> getAllByNameHandler,
        IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse> getAllRevisionsHandler,
        IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse> getRevisionHandler,
        IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse> updateHandler
        )
    {
        _logger = logger;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
        _existsHandler = existsHandler;
        _getHandler = getHandler;
        _getAllHandler = getAllHandler;
        _getAllByNameHandler = getAllByNameHandler;
        _getAllRevisionsHandler = getAllRevisionsHandler;
        _getRevisionHandler = getRevisionHandler;
        _updateHandler = updateHandler;
    }

    public override async Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for CreateCategory");
        return await _createHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<DeleteCategoryResponse> DeleteCategory(DeleteCategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for DeleteCategory");
        return await _deleteHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryExistsResponse> CategoryExists(CategoryExistsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for CategoryExists");
        return await _existsHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<GetCategoryResponse> GetCategory(GetCategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategory");
        return await _getHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<GetAllCategoriesResponse> GetAllCategories(GetAllCategoriesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetAllCategories");
        return await _getAllHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<GetAllCategoriesByNameResponse> GetAllCategoriesByName(GetAllCategoriesByNameRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetAllCategoriesByName");
        return await _getAllByNameHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<GetAllCategoryRevisionsResponse> GetAllCategoryRevisions(GetAllCategoryRevisionsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategoryRevision");
        return await _getAllRevisionsHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<GetCategoryRevisionResponse> GetCategoryRevision(GetCategoryRevisionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategoryRevision");
        return await _getRevisionHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<UpdateCategoryResponse> UpdateCategory(UpdateCategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for UpdateCategory");
        return await _updateHandler.HandleAsync(request, context.CancellationToken);
    }
}
