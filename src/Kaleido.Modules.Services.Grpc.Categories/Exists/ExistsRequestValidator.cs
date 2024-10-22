using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Exists;

public class ExistsRequestValidator : IRequestValidator<CategoryExistsRequest>
{
    private readonly ICategoryValidator _validator;

    public ExistsRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public async Task<ValidationResult> ValidateAsync(CategoryExistsRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        if (request == null)
        {
            validationResult.AddRequiredError([nameof(request)], "Request is required");
            return validationResult;
        }

        var keyValidationResult = await _validator.ValidateKeyFormatAsync(request.Key, cancellationToken);

        if (!keyValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(keyValidationResult.PrependPath([nameof(request.Key)]));
        }

        return validationResult;
    }
}
