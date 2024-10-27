using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public class GetHandler : IGetHandler
{
    private readonly IGetManager _manager;
    private readonly ILogger<GetHandler> _logger;
    public readonly CategoryRequestValidator _validator;
    public readonly IMapper _mapper;

    public GetHandler(
        IGetManager manager,
        CategoryRequestValidator validator,
        ILogger<GetHandler> logger,
        IMapper mapper
    )
    {
        _manager = manager;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> HandleAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCategory request for key: {Key}", request.Key);

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? category;

        try
        {
            _validator.ValidateAndThrow(request);
            category = await _manager.GetAsync(request.Key, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for get category. Key: {Key}. Errors: {Errors}", request.Key, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (category == null)
        {
            _logger.LogWarning("Category with key {Key} not found", request.Key);
            throw new RpcException(new Status(StatusCode.NotFound, "Category not found"));
        }

        return _mapper.Map<CategoryResponse>(category);
    }
}
