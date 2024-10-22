using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;

public interface ICategoryValidator
{
    Task<ValidationResult> ValidateCreateAsync(CreateCategory category, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateUpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateKeyFormatAsync(string key, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateCategoryNameAsync(string name, CancellationToken cancellationToken = default);
}
