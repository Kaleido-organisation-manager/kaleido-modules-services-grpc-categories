using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Mappers;

public class CategoryMapper : ICategoryMapper
{
    public Category ToCategory(CategoryEntity categoryEntity)
    {
        return new Category
        {
            Key = categoryEntity.Key.ToString(),
            Name = categoryEntity.Name,
        };
    }

    public CategoryRevision ToCategoryRevision(CategoryEntity categoryEntity)
    {
        return new CategoryRevision
        {
            Key = categoryEntity.Key.ToString(),
            Name = categoryEntity.Name,
            CreatedAt = categoryEntity.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Revision = categoryEntity.Revision,
            Status = categoryEntity.Status.ToString(),
        };
    }

    public CategoryEntity ToCreateEntity(CreateCategory createCategory)
    {
        return new CategoryEntity
        {
            Id = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Name = createCategory.Name,
            Revision = 1,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public CategoryEntity ToEntity(Category category, int? newRevision = null)
    {
        return new CategoryEntity
        {
            Id = Guid.NewGuid(),
            Key = Guid.Parse(category.Key),
            Name = category.Name,
            Revision = newRevision ?? 1,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
