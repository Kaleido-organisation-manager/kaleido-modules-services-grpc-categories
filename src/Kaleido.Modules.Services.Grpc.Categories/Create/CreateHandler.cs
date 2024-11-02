using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public class CreateHandler : ICreateHandler
{
    private readonly ICreateManager _createManager;
    private readonly ILogger<CreateHandler> _logger;
    private readonly IMapper _mapper;
    public readonly CategoryValidator _validator;

    public CreateHandler(
        ICreateManager createManager,
        ILogger<CreateHandler> logger,
        IMapper mapper,
        CategoryValidator validator
        )
    {
        _createManager = createManager;
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<CategoryResponse> HandleAsync(Category request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling CreateCategory request for name: {Name}", request.Name);

        try
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var category = _mapper.Map<CategoryEntity>(request);
            var result = await _createManager.CreateAsync(category, cancellationToken);

            return _mapper.Map<CategoryResponse>(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed for category with name: {Name}. Errors: {Errors}", request.Name, ex.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating category with name: {Name}", request.Name);
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}