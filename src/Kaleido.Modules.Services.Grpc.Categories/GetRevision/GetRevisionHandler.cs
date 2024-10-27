using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionHandler : IGetRevisionHandler
{
    private readonly IGetRevisionManager _manager;
    private readonly ILogger<GetRevisionHandler> _logger;
    private readonly GetCategoryRevisionRequestValidator _validator;
    private readonly IMapper _mapper;

    public GetRevisionHandler(
        IGetRevisionManager manager,
        ILogger<GetRevisionHandler> logger,
        GetCategoryRevisionRequestValidator validator,
        IMapper mapper
    )
    {
        _manager = manager;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> HandleAsync(GetCategoryRevisionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCategoryRevision request for category with key: {Key} and revision: {Revision}", request.Key, request.Revision);

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? result;

        try
        {
            _validator.ValidateAndThrow(request);
            result = await _manager.GetRevisionAsync(request.Key, request.Revision, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for get category revision. Key: {Key}. Revision {Revision}. Errors: {Errors}", request.Key, request.Revision, ex.Errors.Select(e => e.ErrorMessage));
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
