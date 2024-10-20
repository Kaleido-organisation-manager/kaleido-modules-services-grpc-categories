using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public class GetAllRevisionsHandler : IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse>
{
    private readonly IGetAllRevisionsManager _manager;
    private readonly ILogger<GetAllRevisionsHandler> _logger;
    public IRequestValidator<GetAllCategoryRevisionsRequest> Validator { get; }

    public GetAllRevisionsHandler(
        IGetAllRevisionsManager manager,
        ILogger<GetAllRevisionsHandler> logger,
        IRequestValidator<GetAllCategoryRevisionsRequest> validator
        )
    {
        _manager = manager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<GetAllCategoryRevisionsResponse> HandleAsync(GetAllCategoryRevisionsRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetAllRevisions request for category with key: {Key}", request.Key);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        try
        {
            var revisions = await _manager.HandleAsync(request.Key, cancellationToken);
            return new GetAllCategoryRevisionsResponse { Revisions = { revisions } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetAllRevisions request for category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
