using FluentValidation;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public class GetCategoryRevisionRequestValidator : AbstractValidator<GetCategoryRevisionRequest>
{
    public GetCategoryRevisionRequestValidator()
    {
        RuleFor(x => x.Key).SetValidator(new KeyValidator());
        RuleFor(x => x.Revision).NotNull().GreaterThan(0);
    }
}