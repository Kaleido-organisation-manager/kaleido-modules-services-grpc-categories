using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Exists;

public class ExistsHandler : IBaseHandler<CategoryExistsRequest, CategoryExistsResponse>
{
    private readonly IExistsManager _manager;
    private readonly ILogger<ExistsHandler> _logger;
    public IRequestValidator<CategoryExistsRequest> Validator { get; }

    public ExistsHandler(
        IExistsManager manager,
        ILogger<ExistsHandler> logger,
        IRequestValidator<CategoryExistsRequest> validator
    )
    {
        _manager = manager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<CategoryExistsResponse> HandleAsync(CategoryExistsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CategoryExists request for key: {Key}", request.Key);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        try
        {
            var exists = await _manager.ExistsAsync(request.Key, cancellationToken);
            return new CategoryExistsResponse { Exists = exists };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while checking if category exists with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
