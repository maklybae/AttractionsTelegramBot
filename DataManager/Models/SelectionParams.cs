using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

public class SelectionParams
{
    [Column("selection_params_id"), Key]
    public int SelectionParamsId {  get; set; }

    [Column("selection_id")]
    public Selection Selection { get; set; }

    [Column("field")]
    public int Field { get; set; }

    [Column("value")]
    public string? Value { get; set; }
}

