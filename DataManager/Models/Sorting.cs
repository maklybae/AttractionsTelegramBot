using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Models;

public class Sorting
{
    [Column("sorting_id"), Key]
    public int SortingId { get; set; }

    [Column("chat_id")]
    public Chat Chat { get; set; }

    [Column("file")]
    public int? IdentNumberFile { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("is_json")]
    public bool? IsJson { get; set; }
}
