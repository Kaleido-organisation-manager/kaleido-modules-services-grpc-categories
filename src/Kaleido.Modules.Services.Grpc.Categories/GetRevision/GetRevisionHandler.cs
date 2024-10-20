using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionHandler : IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse>
{
    private readonly IGetRevisionManager _manager;
    private readonly ILogger<GetRevisionHandler> _logger;
    public IRequestValidator<GetCategoryRevisionRequest> Validator { get; }

    public GetRevisionHandler(
        IGetRevisionManager manager,
        ILogger<GetRevisionHandler> logger,
        IRequestValidator<GetCategoryRevisionRequest> validator
    )
    {
        _manager = manager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<GetCategoryRevisionResponse> HandleAsync(GetCategoryRevisionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCategoryRevision request for category with key: {Key} and revision: {Revision}", request.Key, request.Revision);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        CategoryRevision? categoryRevision;

        try
        {
            categoryRevision = await _manager.GetRevisionAsync(request.Key, request.Revision, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category revision");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (categoryRevision == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Category revision not found"));
        }

        return new GetCategoryRevisionResponse { Revision = categoryRevision };
    }
}
