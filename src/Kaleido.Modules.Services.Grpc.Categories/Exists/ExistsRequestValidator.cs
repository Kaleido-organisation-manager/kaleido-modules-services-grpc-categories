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

    public Task<ValidationResult> ValidateAsync(CategoryExistsRequest request, CancellationToken cancellationToken = default)
    {
        return _validator.ValidateKeyFormatAsync(request.Key, cancellationToken);
    }
}
