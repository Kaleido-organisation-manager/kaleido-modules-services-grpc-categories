using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllRequestValidator : IRequestValidator<GetAllCategoriesRequest>
{
    public Task<ValidationResult> ValidateAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Task.FromResult(new ValidationResult().AddRequiredError([nameof(request)], "Request is required"));
        }

        return Task.FromResult(new ValidationResult());
    }
}