using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;

public interface ICategoryMapper
{
    Category ToCategory(CategoryEntity categoryEntity);
    CategoryEntity ToCreateEntity(CreateCategory createCategory);
    CategoryRevision ToCategoryRevision(CategoryEntity categoryEntity);
    CategoryEntity ToEntity(Category category, int? newRevision = null);

}