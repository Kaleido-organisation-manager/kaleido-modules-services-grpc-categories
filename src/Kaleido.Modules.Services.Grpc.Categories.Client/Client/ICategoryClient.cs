using Kaleido.Modules.Services.Grpc.Categories.Client.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Client.Client;

/// <summary>
/// Client interface for interacting with the Category service
/// </summary>
public interface ICategoryClient
{
    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="name">The name of the category</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The created category details</returns>
    Task<CategoryDto> CreateAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new category using a Category object
    /// </summary>
    /// <param name="category">The Category object to create</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The created category details</returns>
    Task<CategoryDto> CreateAsync(CategoryEntityDto category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="key">The key of the category to update</param>
    /// <param name="name">The new name for the category</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The updated category details</returns>
    Task<CategoryDto> UpdateAsync(Guid key, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category using a Category object
    /// </summary>
    /// <param name="key">The key of the category to update</param>
    /// <param name="category">The Category object containing the updates</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The updated category details</returns>
    Task<CategoryDto> UpdateAsync(Guid key, CategoryEntityDto category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="key">The key of the category to delete</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The deleted category details</returns>
    Task<CategoryDto> DeleteAsync(Guid key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific category
    /// </summary>
    /// <param name="key">The key of the category</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The category details</returns>
    Task<CategoryDto> GetAsync(Guid key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of category details</returns>
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories filtered by name
    /// </summary>
    /// <param name="name">The name to filter by</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of filtered category details</returns>
    Task<IEnumerable<CategoryDto>> GetAllFilteredAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all revisions for a specific category
    /// </summary>
    /// <param name="key">The key of the category</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of category revisions</returns>
    Task<IEnumerable<CategoryDto>> GetAllRevisionsAsync(Guid key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific revision of a category
    /// </summary>
    /// <param name="key">The key of the category</param>
    /// <param name="createdAt">The timestamp of the revision</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The category revision details</returns>
    Task<CategoryDto> GetRevisionAsync(Guid key, DateTime createdAt, CancellationToken cancellationToken = default);
}