using FluentValidation;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name).SetValidator(new NameValidator());
    }
}
