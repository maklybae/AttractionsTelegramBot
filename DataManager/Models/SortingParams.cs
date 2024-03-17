using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Models;

public class SortingParams
{
    [Column("sorting_params_id"), Key]
    public int SortingParamsId { get; set; }

    [Column("selection_id")]
    public Sorting Sorting { get; set; }

    [Column("field")]
    public int Field { get; set; }

    [Column("is_descending")]
    public bool? IsDescending { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
