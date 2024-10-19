using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public class UpdateHandler : IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse>
{
    private readonly IUpdateManager _updateManager;
    private readonly ILogger<UpdateHandler> _logger;
    public IRequestValidator<UpdateCategoryRequest> Validator { get; }

    public UpdateHandler(
        IUpdateManager updateManager,
        ILogger<UpdateHandler> logger,
        IRequestValidator<UpdateCategoryRequest> validator
    )
    {
        _updateManager = updateManager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<UpdateCategoryResponse> HandleAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCategory request with key: {Key}", request.Key);
        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        if (!request.Key.Equals(request.Category.Key))
        {
            validationResult.AddInvalidFormatError([nameof(request.Key)], "Key in request does not match key in category");
        }
        validationResult.ThrowIfInvalid();

        Category? updatedCategory;

        try
        {
            updatedCategory = await _updateManager.UpdateAsync(request.Category, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (updatedCategory == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Category with key {request.Key} not found"));
        }

        return new UpdateCategoryResponse { Category = updatedCategory };
    }
}
