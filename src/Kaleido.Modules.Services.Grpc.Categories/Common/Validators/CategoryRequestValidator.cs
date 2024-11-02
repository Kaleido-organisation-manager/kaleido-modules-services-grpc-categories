using FluentValidation;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Key).SetValidator(new KeyValidator());
    }
}