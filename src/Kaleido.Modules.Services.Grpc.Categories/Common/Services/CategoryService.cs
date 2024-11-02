using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using static Kaleido.Grpc.Categories.GrpcCategories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Services;

public class CategoryService : GrpcCategoriesBase
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ICreateHandler _createHandler;
    private readonly IDeleteHandler _deleteHandler;
    private readonly IGetHandler _getHandler;
    private readonly IGetAllHandler _getAllHandler;
    private readonly IGetAllByNameHandler _getAllByNameHandler;
    private readonly IGetAllRevisionsHandler _getAllRevisionsHandler;
    private readonly IGetRevisionHandler _getRevisionHandler;
    private readonly IUpdateHandler _updateHandler;

    public CategoryService(
        ILogger<CategoryService> logger,
        ICreateHandler createHandler,
        IDeleteHandler deleteHandler,
        IGetHandler getHandler,
        IGetAllHandler getAllHandler,
        IGetAllByNameHandler getAllByNameHandler,
        IGetAllRevisionsHandler getAllRevisionsHandler,
        IGetRevisionHandler getRevisionHandler,
        IUpdateHandler updateHandler
        )
    {
        _logger = logger;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
        // _existsHandler = existsHandler;
        _getHandler = getHandler;
        _getAllHandler = getAllHandler;
        _getAllByNameHandler = getAllByNameHandler;
        _getAllRevisionsHandler = getAllRevisionsHandler;
        _getRevisionHandler = getRevisionHandler;
        _updateHandler = updateHandler;
    }

    public override async Task<CategoryResponse> CreateCategory(Category request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for CreateCategory");
        return await _createHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryResponse> DeleteCategory(CategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for DeleteCategory");
        return await _deleteHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryResponse> GetCategory(CategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategory");
        return await _getHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryListResponse> GetAllCategories(EmptyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetAllCategories");
        return await _getAllHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryListResponse> GetAllCategoriesByName(GetAllCategoriesByNameRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetAllCategoriesByName");
        return await _getAllByNameHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryListResponse> GetAllCategoryRevisions(CategoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategoryRevision");
        return await _getAllRevisionsHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryResponse> GetCategoryRevision(GetCategoryRevisionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for GetCategoryRevision");
        return await _getRevisionHandler.HandleAsync(request, context.CancellationToken);
    }

    public override async Task<CategoryResponse> UpdateCategory(CategoryActionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request received for UpdateCategory");
        return await _updateHandler.HandleAsync(request, context.CancellationToken);
    }
}
