namespace Kaleido.Modules.Services.Grpc.Categories.Client.Models;

/// <summary>
/// Data transfer object representing a category entity
/// </summary>
public class CategoryEntityDto
{
    /// <summary>
    /// The name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
}