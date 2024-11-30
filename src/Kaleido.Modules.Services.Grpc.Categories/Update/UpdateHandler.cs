using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public class UpdateHandler : IUpdateHandler
{
    private readonly IUpdateManager _updateManager;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly KeyValidator _keyValidator;
    private readonly CategoryValidator _categoryValidator;
    private readonly IMapper _mapper;

    public UpdateHandler(
        IUpdateManager updateManager,
        ILogger<UpdateHandler> logger,
        KeyValidator keyValidator,
        CategoryValidator categoryValidator,
        IMapper mapper
    )
    {
        _updateManager = updateManager;
        _logger = logger;
        _keyValidator = keyValidator;
        _categoryValidator = categoryValidator;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> HandleAsync(CategoryActionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling UpdateCategory request with key: {Key}", request.Key);

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? updateResult;

        try
        {
            await _keyValidator.ValidateAndThrowAsync(request.Key, cancellationToken);
            await _categoryValidator.ValidateAndThrowAsync(request.Category, cancellationToken);
            var category = _mapper.Map<CategoryEntity>(request.Category);
            updateResult = await _updateManager.UpdateAsync(Guid.Parse(request.Key), category, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for update category. Key: {Key}. Errors: {Errors}", request.Key, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "An error occured while updating category with key: {Key}", request.Key);
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }

        if (updateResult == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Category with key {request.Key} not found"));
        }

        return _mapper.Map<CategoryResponse>(updateResult);
    }
}
