using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public class GetAllRevisionsRequestValidator : IRequestValidator<GetAllCategoryRevisionsRequest>
{
    private readonly ICategoryValidator _validator;

    public GetAllRevisionsRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public async Task<ValidationResult> ValidateAsync(GetAllCategoryRevisionsRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        var keyValidationResult = await _validator.ValidateKeyFormatAsync(request.Key, cancellationToken);
        if (!keyValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(keyValidationResult);
        }

        return validationResult;
    }
}