using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

public class Chat
{
    [Column("chat_id"), Key]
    public int ChatId { get; set; }

    [Column("status")]
    public int Status { get; set; }
}
