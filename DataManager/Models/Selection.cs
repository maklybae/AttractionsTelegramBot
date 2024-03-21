using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

/// <summary>
/// Represents a selection entity.
/// </summary>
public class Selection
{
    /// <summary>
    /// Gets or sets the selection ID.
    /// </summary>
    [Column("selection_id"), Key]
    public int SelectionId { get; set; }

    /// <summary>
    /// Gets or sets the chat associated with the selection.
    /// </summary>
    [Column("chat_id")]
    public Chat Chat { get; set; }

    /// <summary>
    /// Gets or sets the identification number of the file associated with the selection.
    /// </summary>
    [Column("file")]
    public int? IdentNumberFile { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the selection was created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is in JSON format.
    /// </summary>
    [Column("is_json")]
    public bool? IsJson { get; set; }
}
