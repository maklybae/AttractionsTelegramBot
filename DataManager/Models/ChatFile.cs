using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

public class ChatFile
{
    [Column("file_id"), Key]
    public string ChatFileId { get; set; }

    [Column("chat_id")]
    public Chat Chat { get; set; }
}
