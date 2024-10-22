using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteRequestValidator : IRequestValidator<DeleteCategoryRequest>
{
    private readonly ICategoryValidator _validator;

    public DeleteRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public async Task<ValidationResult> ValidateAsync(DeleteCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        if (request is null)
        {
            validationResult.AddRequiredError([nameof(request)], "Request is required");
            return validationResult;
        }

        var keyValidationResult = await _validator.ValidateKeyFormatAsync(request.Key, cancellationToken);

        if (!keyValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(keyValidationResult.PrependPath([nameof(request)]));
        }

        return validationResult;
    }
}