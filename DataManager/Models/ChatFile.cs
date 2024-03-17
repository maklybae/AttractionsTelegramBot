using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

public class ChatFile
{
    [Column("file_id"), Key]
    public int FileId { get; set; }

    [Column("telegram_file_id")]
    public string ChatFileId { get; set; }

    [Column("chat_id")]
    public Chat Chat { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("is_source")]
    public bool IsSource { get; set; }
}
