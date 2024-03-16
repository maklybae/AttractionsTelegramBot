using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

public class Selection
{
    [Column("selection_id"), Key]
    public int SelectionId { get; set; }

    [Column("chat_id")]
    public Chat Chat { get; set; }

    [Column("source_file")]
    public ChatFile? SourceFile { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
