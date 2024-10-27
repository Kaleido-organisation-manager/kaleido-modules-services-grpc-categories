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
    private readonly CategoryActionValidator _validator;
    private readonly IMapper _mapper;

    public UpdateHandler(
        IUpdateManager updateManager,
        ILogger<UpdateHandler> logger,
        CategoryActionValidator validator,
        IMapper mapper
    )
    {
        _updateManager = updateManager;
        _logger = logger;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> HandleAsync(CategoryActionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling UpdateCategory request with key: {Key}", request.Key);

        EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>? updateResult;

        try
        {
            _validator.ValidateAndThrow(request);
            var category = _mapper.Map<CategoryEntity>(request.Category);
            updateResult = await _updateManager.UpdateAsync(Guid.Parse(request.Key), category, cancellationToken);
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
