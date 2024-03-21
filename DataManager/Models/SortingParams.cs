using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Models;

/// <summary>
/// Represents sorting parameters entity.
/// </summary>
public class SortingParams
{
    /// <summary>
    /// Gets or sets the sorting parameters ID.
    /// </summary>
    [Column("sorting_params_id"), Key]
    public int SortingParamsId { get; set; }

    /// <summary>
    /// Gets or sets the sorting associated with the parameters.
    /// </summary>
    [Column("selection_id")]
    public Sorting Sorting { get; set; }

    /// <summary>
    /// Gets or sets the field value of the sorting parameters.
    /// </summary>
    [Column("field")]
    public int Field { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sorting is in descending order.
    /// </summary>
    [Column("is_descending")]
    public bool? IsDescending { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sorting parameters were created.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
