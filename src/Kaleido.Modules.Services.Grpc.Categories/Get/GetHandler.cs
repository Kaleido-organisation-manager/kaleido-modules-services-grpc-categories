using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public class GetHandler : IBaseHandler<GetCategoryRequest, GetCategoryResponse>
{
    private readonly IGetManager _manager;
    public IRequestValidator<GetCategoryRequest> Validator { get; }
    private readonly ILogger<GetHandler> _logger;

    public GetHandler(
        IGetManager manager,
        IRequestValidator<GetCategoryRequest> validator,
        ILogger<GetHandler> logger
    )
    {
        _manager = manager;
        Validator = validator;
        _logger = logger;
    }

    public async Task<GetCategoryResponse> HandleAsync(GetCategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCategory request for key: {Key}", request.Key);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        Category? category;

        try
        {
            category = await _manager.GetAsync(request.Key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (category == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Category not found"));
        }

        return new GetCategoryResponse { Category = category };
    }
}
