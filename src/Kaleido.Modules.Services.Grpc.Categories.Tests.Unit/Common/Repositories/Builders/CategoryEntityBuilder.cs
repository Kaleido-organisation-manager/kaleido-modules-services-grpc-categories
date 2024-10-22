using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Repositories.Builders;

public class CategoryEntityBuilder
{
    private CategoryEntity _category { get; set; }

    public CategoryEntityBuilder()
    {
        _category = new CategoryEntity()
        {
            Name = "Test Category",
            Key = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Revision = 1,
            Status = EntityStatus.Active
        };
    }

    public CategoryEntityBuilder WithName(string name)
    {
        _category.Name = name;
        return this;
    }

    public CategoryEntityBuilder WithStatus(EntityStatus status)
    {
        _category.Status = status;
        return this;
    }

    public CategoryEntity Build()
    {
        return _category;
    }
}