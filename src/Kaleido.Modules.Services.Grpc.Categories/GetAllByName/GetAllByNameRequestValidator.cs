using FluentValidation;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public class GetAllByNameRequestValidator : AbstractValidator<GetAllCategoriesByNameRequest>
{
    public GetAllByNameRequestValidator()
    {
        RuleFor(x => x.Name).SetValidator(new NameValidator());
    }
}