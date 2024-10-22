using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public class CreateHandler : IBaseHandler<CreateCategoryRequest, CreateCategoryResponse>
{
    private readonly ICreateManager _createManager;
    private readonly ILogger<CreateHandler> _logger;
    public IRequestValidator<CreateCategoryRequest> Validator { get; }

    public CreateHandler(
        ICreateManager createManager,
        ILogger<CreateHandler> logger,
        IRequestValidator<CreateCategoryRequest> validator)
    {
        _createManager = createManager;
        _logger = logger;
        Validator = validator;
    }

    public async Task<CreateCategoryResponse> HandleAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling CreateCategory request for name: {Name}", request.Category.Name);

        var validationResult = await Validator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        try
        {
            var category = await _createManager.CreateAsync(request.Category, cancellationToken);

            return new CreateCategoryResponse { Category = category };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating category with name: {Name}", request.Category.Name);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}