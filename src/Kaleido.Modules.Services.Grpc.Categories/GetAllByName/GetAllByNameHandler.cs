using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameHandler : IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse>
{
    private readonly IGetAllByNameManager _manager;
    private readonly ILogger<GetAllByNameHandler> _logger;
    public IRequestValidator<GetAllCategoriesByNameRequest> Validator { get; }

    public GetAllByNameHandler(
        IGetAllByNameManager manager,
        ILogger<GetAllByNameHandler> logger,
        IRequestValidator<GetAllCategoriesByNameRequest> validator
        )
    {
        _manager = manager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<GetAllCategoriesByNameResponse> HandleAsync(GetAllCategoriesByNameRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllCategoriesByName request for name: {name}", request.Name);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        try
        {
            var categories = await _manager.GetAllByNameAsync(request.Name, cancellationToken);
            return new GetAllCategoriesByNameResponse { Categories = { categories.ToList() } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories by name: {name}", request.Name);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
