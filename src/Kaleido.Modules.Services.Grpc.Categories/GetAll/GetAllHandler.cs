using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllHandler : IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse>
{
    private readonly IGetAllManager _manager;
    private readonly ILogger<GetAllHandler> _logger;
    public IRequestValidator<GetAllCategoriesRequest> Validator { get; }

    public GetAllHandler(
        IGetAllManager manager,
        ILogger<GetAllHandler> logger,
        IRequestValidator<GetAllCategoriesRequest> validator
    )
    {
        _manager = manager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<GetAllCategoriesResponse> HandleAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetAllCategories request");

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        try
        {
            var categories = await _manager.GetAllAsync(cancellationToken);
            return new GetAllCategoriesResponse { Categories = { categories.ToList() } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetAllCategories request");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
