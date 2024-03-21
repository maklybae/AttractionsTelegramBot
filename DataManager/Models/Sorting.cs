using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Models;

/// <summary>
/// Represents a sorting entity.
/// </summary>
public class Sorting
{
    /// <summary>
    /// Gets or sets the sorting ID.
    /// </summary>
    [Column("sorting_id"), Key]
    public int SortingId { get; set; }

    /// <summary>
    /// Gets or sets the chat associated with the sorting.
    /// </summary>
    [Column("chat_id")]
    public Chat Chat { get; set; }

    /// <summary>
    /// Gets or sets the identification number of the file associated with the sorting.
    /// </summary>
    [Column("file")]
    public int? IdentNumberFile { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sorting was created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sorting is in JSON format.
    /// </summary>
    [Column("is_json")]
    public bool? IsJson { get; set; }
}
