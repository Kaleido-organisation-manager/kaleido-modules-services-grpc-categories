using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Kaleido.Grpc.Categories;
using Kaleido.Common.Services.Grpc.Models.Validations;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

public class CategoryValidator : ICategoryValidator
{

    public Task<ValidationResult> ValidateCreateAsync(CreateCategory createCategory, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Name = createCategory.Name,
        };

        return ValidateCommonRulesAsync(category, cancellationToken);
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();
        var keyValidationResult = await ValidateKeyFormatAsync(category.Key, cancellationToken);

        if (!keyValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(keyValidationResult.PrependPath([nameof(Category)]));
        }

        var commonValidationResult = await ValidateCommonRulesAsync(category, cancellationToken);

        if (!commonValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(commonValidationResult.PrependPath([nameof(Category)]));
        }

        return validationResult;
    }

    private async Task<ValidationResult> ValidateCommonRulesAsync(Category category, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        var nameValidationResult = await ValidateCategoryNameAsync(category.Name, cancellationToken);

        if (!nameValidationResult.IsValid)
        {
            validationResult = validationResult.Merge(nameValidationResult.PrependPath([nameof(Category)]));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateKeyFormatAsync(string key, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(key))
        {
            validationResult.AddInvalidFormatError([nameof(key)], "Key is required");
        }
        if (!Guid.TryParse(key, out var guid))
        {
            validationResult.AddInvalidFormatError([nameof(key)], "Key is not a valid GUID");
        }

        return Task.FromResult(validationResult);
    }

    public Task<ValidationResult> ValidateCategoryNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var validationResult = new ValidationResult();


        if (string.IsNullOrEmpty(name))
        {
            validationResult.AddInvalidFormatError([nameof(name)], "Name is required");
        }

        if (name.Length >= 100)
        {
            validationResult.AddInvalidFormatError([nameof(name)], "Name cannot be longer than 100 characters");
        }

        return Task.FromResult(validationResult);
    }
}
