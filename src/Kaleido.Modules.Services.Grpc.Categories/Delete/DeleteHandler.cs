using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteHandler : IDeleteHandler
{
    private readonly IDeleteManager _deleteManager;
    private readonly ILogger<DeleteHandler> _logger;
    private readonly CategoryRequestValidator _validator;
    private readonly IMapper _mapper;

    public DeleteHandler(
        IDeleteManager deleteManager,
        ILogger<DeleteHandler> logger,
        CategoryRequestValidator validator,
        IMapper mapper
        )
    {
        _deleteManager = deleteManager;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }


    public async Task<CategoryResponse> HandleAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling DeleteCategory request for key: {Key}", request.Key);

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? entity;
        try
        {
            _validator.ValidateAndThrow(request);
            entity = await _deleteManager.DeleteCategoryAsync(request.Key, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for category deletion. Key: {Key}. Errors: {Errors}", request.Key, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while deleting category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (entity == null)
        {
            _logger.LogWarning("Category with key {Key} not found", request.Key);
            throw new RpcException(new Status(StatusCode.NotFound, $"Could not find category with key {request.Key}"));
        }

        return _mapper.Map<CategoryResponse>(entity);
    }
}
