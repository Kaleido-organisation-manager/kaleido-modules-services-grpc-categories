using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Client.Models;

/// <summary>
/// Data transfer object representing revision information for a category
/// </summary>
public class CategoryRevisionDto
{
    /// <summary>
    /// The unique identifier of the revision
    /// </summary>
    public Guid Key { get; set; }

    /// <summary>
    /// The revision number
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// The action performed in this revision (e.g., Created, Updated, Deleted)
    /// </summary>
    public RevisionAction Action { get; set; }

    /// <summary>
    /// When this revision was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The status of this revision
    /// </summary>
    public RevisionStatus Status { get; set; }
}