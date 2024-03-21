using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models;

/// <summary>
/// Represents selection parameters entity.
/// </summary>
public class SelectionParams
{
    /// <summary>
    /// Gets or sets the selection parameters ID.
    /// </summary>
    [Column("selection_params_id"), Key]
    public int SelectionParamsId {  get; set; }

    /// <summary>
    /// Gets or sets the selection associated with the parameters.
    /// </summary>
    [Column("selection_id")]
    public Selection Selection { get; set; }

    /// <summary>
    /// Gets or sets the field value of the parameters.
    /// </summary>
    [Column("field")]
    public int Field { get; set; }

    /// <summary>
    /// Gets or sets the value of the parameters.
    /// </summary>
    [Column("value")]
    public string? Value { get; set; }
}

