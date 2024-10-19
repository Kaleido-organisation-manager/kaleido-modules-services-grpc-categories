using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteHandler : IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse>
{
    private readonly IDeleteManager _deleteManager;
    private readonly ILogger<DeleteHandler> _logger;
    public IRequestValidator<DeleteCategoryRequest> Validator { get; }

    public DeleteHandler(
        IDeleteManager deleteManager,
        ILogger<DeleteHandler> logger,
        IRequestValidator<DeleteCategoryRequest> validator
        )
    {
        _deleteManager = deleteManager;
        _logger = logger;
        Validator = validator;
    }


    public async Task<DeleteCategoryResponse> HandleAsync(DeleteCategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling DeleteCategory request for key: {Key}", request.Key);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        CategoryEntity? deletedEntity;

        try
        {
            deletedEntity = await _deleteManager.DeleteCategoryAsync(request.Key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while deleting category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (deletedEntity == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Category with key: {request.Key} not found"));
        }

        return new DeleteCategoryResponse()
        {
            Key = request.Key
        };
    }
}
