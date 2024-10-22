using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Models;

public class CategoryEntity : BaseEntity
{
    public required string Name { get; set; }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is CategoryEntity other && other.Name == Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name);
    }
}
