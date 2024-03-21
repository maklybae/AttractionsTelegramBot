using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

/// <summary>
/// Represents a chat file entity.
/// </summary>
public class ChatFile
{
    /// <summary>
    /// Gets or sets the file ID.
    /// </summary>
    [Column("file_id"), Key]
    public int FileId { get; set; }

    /// <summary>
    /// Gets or sets the Telegram file ID associated with the chat file.
    /// </summary>
    [Column("telegram_file_id")]
    public string ChatFileId { get; set; }

    /// <summary>
    /// Gets or sets the chat associated with the chat file.
    /// </summary>
    [Column("chat_id")]
    public Chat Chat { get; set; }

    /// <summary>
    /// Gets or sets the description of the chat file.
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the chat file was created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the chat file is a source file.
    /// </summary>
    [Column("is_source")]
    public bool IsSource { get; set; }
}
