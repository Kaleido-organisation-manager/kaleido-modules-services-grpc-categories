using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public class GetAllRevisionsHandler : IGetAllRevisionsHandler
{
    private readonly IGetAllRevisionsManager _manager;
    private readonly ILogger<GetAllRevisionsHandler> _logger;
    private readonly CategoryRequestValidator _validator;
    private readonly IMapper _mapper;

    public GetAllRevisionsHandler(
        IGetAllRevisionsManager manager,
        ILogger<GetAllRevisionsHandler> logger,
        CategoryRequestValidator validator,
        IMapper mapper
        )
    {
        _manager = manager;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<CategoryListResponse> HandleAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetAllRevisions request for category with key: {Key}", request.Key);

        try
        {
            _validator.ValidateAndThrow(request);
            var revisions = await _manager.HandleAsync(request.Key, cancellationToken);
            return _mapper.Map<CategoryListResponse>(revisions);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for get all category revisions. Key: {Key}. Errors: {Errors}", request.Key, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetAllRevisions request for category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
