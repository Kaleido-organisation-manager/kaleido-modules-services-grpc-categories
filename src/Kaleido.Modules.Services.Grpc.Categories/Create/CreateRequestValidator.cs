using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public class CreateRequestValidator : IRequestValidator<CreateCategoryRequest>
{
    private readonly ICategoryValidator _categoryValidator;

    public CreateRequestValidator(ICategoryValidator categoryValidator)
    {
        _categoryValidator = categoryValidator;
    }

    public async Task<ValidationResult> ValidateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        if (request == null)
        {
            validationResult.AddRequiredError([nameof(request)], "Request is required");
            return validationResult;
        }
        if (request.Category == null)
        {
            validationResult.AddRequiredError([nameof(request.Category)], "Category is required");
            return validationResult;
        }

        var categoryValidationResult = await _categoryValidator.ValidateCreateAsync(request.Category, cancellationToken);

        if (!categoryValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(categoryValidationResult);
        }

        return validationResult;
    }
}