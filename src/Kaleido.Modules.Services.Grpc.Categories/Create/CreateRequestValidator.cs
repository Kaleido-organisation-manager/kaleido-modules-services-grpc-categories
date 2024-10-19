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

    public Task<ValidationResult> ValidateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return _categoryValidator.ValidateCreateAsync(request.Category, cancellationToken);
    }
}