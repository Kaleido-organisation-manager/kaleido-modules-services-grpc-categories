using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameHandler : IGetAllByNameHandler
{
    private readonly IGetAllByNameManager _manager;
    private readonly ILogger<GetAllByNameHandler> _logger;
    private readonly GetAllByNameRequestValidator _validator;
    private readonly IMapper _mapper;

    public GetAllByNameHandler(
        IGetAllByNameManager manager,
        ILogger<GetAllByNameHandler> logger,
        GetAllByNameRequestValidator validator,
        IMapper mapper
        )
    {
        _manager = manager;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<CategoryListResponse> HandleAsync(GetAllCategoriesByNameRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetAllCategoriesByName request for name: {name}", request.Name);

        try
        {
            _validator.ValidateAndThrow(request);
            var categories = await _manager.GetAllByNameAsync(request.Name, cancellationToken);
            return _mapper.Map<CategoryListResponse>(categories);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for get all category by name. Name: {Name}. Errors: {Errors}", request.Name, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories by name: {name}", request.Name);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
