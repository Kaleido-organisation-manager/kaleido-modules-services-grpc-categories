using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllRequestValidator : IRequestValidator<GetAllCategoriesRequest>
{
    public Task<ValidationResult> ValidateAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ValidationResult());
    }
}