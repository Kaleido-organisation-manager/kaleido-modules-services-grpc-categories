namespace Kaleido.Modules.Services.Grpc.Categories.Client.Models;

/// <summary>
/// Data transfer object representing a category with its revision information
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// The unique identifier of the category
    /// </summary>
    public Guid Key { get; set; }

    /// <summary>
    /// The category entity information
    /// </summary>
    public CategoryEntityDto Category { get; set; } = null!;

    /// <summary>
    /// The revision information for this category
    /// </summary>
    public CategoryRevisionDto Revision { get; set; } = null!;
}