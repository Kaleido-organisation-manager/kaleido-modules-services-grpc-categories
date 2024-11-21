using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionHandler : IGetRevisionHandler
{
    private readonly IGetRevisionManager _manager;
    private readonly ILogger<GetRevisionHandler> _logger;
    private readonly KeyValidator _keyValidator;
    private readonly IMapper _mapper;

    public GetRevisionHandler(
        IGetRevisionManager manager,
        ILogger<GetRevisionHandler> logger,
        KeyValidator keyValidator,
        IMapper mapper
    )
    {
        _manager = manager;
        _logger = logger;
        _keyValidator = keyValidator;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> HandleAsync(GetCategoryRevisionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCategoryRevision request for category with key: {Key} and created at: {CreatedAt}", request.Key, request.CreatedAt.ToDateTime());

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? result;

        try
        {
            await _keyValidator.ValidateAndThrowAsync(request.Key, cancellationToken);
            result = await _manager.GetRevisionAsync(request.Key, request.CreatedAt.ToDateTime(), cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for get category revision. Key: {Key}. CreatedAt {CreatedAt}. Errors: {Errors}", request.Key, request.CreatedAt.ToDateTime(), ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category revision");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (result == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Category revision not found"));
        }

        return _mapper.Map<CategoryResponse>(result);
    }
}
