using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameRequestValidator : IRequestValidator<GetAllCategoriesByNameRequest>
{
    private readonly ICategoryValidator _validator;

    public GetAllByNameRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public Task<ValidationResult> ValidateAsync(GetAllCategoriesByNameRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Task.FromResult(new ValidationResult().AddRequiredError([nameof(request)], "Request is required"));
        }

        return _validator.ValidateCategoryNameAsync(request.Name, cancellationToken);
    }
}