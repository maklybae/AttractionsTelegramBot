using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

/// <summary>
/// Represents a chat entity.
/// </summary>
public class Chat
{
    /// <summary>
    /// Gets or sets the chat ID.
    /// </summary>
    [Column("chat_id"), Key]
    public long ChatId { get; set; }

    /// <summary>
    /// Gets or sets the status of the chat.
    /// </summary>
    [Column("status")]
    public int Status { get; set; }
}
