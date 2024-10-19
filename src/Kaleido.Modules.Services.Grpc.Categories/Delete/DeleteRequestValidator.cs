using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public class DeleteRequestValidator : IRequestValidator<DeleteCategoryRequest>
{
    private readonly ICategoryValidator _validator;

    public DeleteRequestValidator(ICategoryValidator validator)
    {
        _validator = validator;
    }

    public Task<ValidationResult> ValidateAsync(DeleteCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return _validator.ValidateKeyFormatAsync(request.Key, cancellationToken);
    }
}