using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;

public class CategoryBuilder
{
    private readonly Category _category = new()
    {
        Name = "Test Category",
    };

    public CategoryBuilder WithName(string name)
    {
        _category.Name = name;
        return this;
    }

    public Category Build()
    {
        return _category;
    }
}
