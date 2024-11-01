using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Builders;

public class CreateCategoryBuilder
{
    private readonly Category _instance = new()
    {
        Name = "Test Category"
    };

    public CreateCategoryBuilder WithName(string name)
    {
        _instance.Name = name;
        return this;
    }

    public Category Build() => _instance;
}