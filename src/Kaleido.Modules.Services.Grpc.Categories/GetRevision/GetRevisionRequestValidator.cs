using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetRevisionRequestValidator : IRequestValidator<GetCategoryRevisionRequest>
{
    private readonly ICategoryValidator _validator;

    public GetRevisionRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public async Task<ValidationResult> ValidateAsync(GetCategoryRevisionRequest request, CancellationToken cancellationToken = default)
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
            validationResult = validationResult.Merge(keyValidationResult);
        }

        if (request.Revision < 1)
        {
            validationResult.AddInvalidFormatError([nameof(request), nameof(request.Revision)], "Revision must be greater than 0");
        }

        return validationResult;
    }
}
