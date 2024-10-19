using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public class UpdateRequestValidator : IRequestValidator<UpdateCategoryRequest>
{
    private readonly ICategoryValidator _validator;

    public UpdateRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public Task<ValidationResult> ValidateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return _validator.ValidateUpdateAsync(request.Category, cancellationToken);
    }
}
