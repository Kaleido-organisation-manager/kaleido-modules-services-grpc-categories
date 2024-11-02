using FluentValidation;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

public class CategoryActionValidator : AbstractValidator<CategoryActionRequest>
{
    public CategoryActionValidator()
    {
        RuleFor(x => x.Key).SetValidator(new KeyValidator());
        RuleFor(x => x.Category).SetValidator(new CategoryValidator());
    }
}