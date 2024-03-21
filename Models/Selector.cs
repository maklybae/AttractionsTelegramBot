namespace Models;

/// <summary>
/// Represents a selector for filtering attractions based on selection parameters.
/// </summary>
public class Selector
{
    private IEnumerable<Attraction> _attractions;
    private IEnumerable<(string field, string value)> _selectionParams;

    /// <summary>
    /// Initializes a new instance of the Selector class with the specified attractions and selection parameters.
    /// </summary>
    /// <param name="attractions">The collection of attractions to filter.</param>
    /// <param name="selectionParams">The collection of selection parameters to apply.</param>
    public Selector(IEnumerable<Attraction> attractions, IEnumerable<(string field, string value)> selectionParams)
    {
        _attractions = attractions;
        _selectionParams = selectionParams;
    }

    /// <summary>
    /// Selects attractions based on the specified selection parameters.
    /// </summary>
    /// <returns>The filtered collection of attractions.</returns>
    public IEnumerable<Attraction> Select()
    {
        foreach ((string field, string value) in _selectionParams)
        {
            int intValue = 0;
            if (field == "global_id")
                int.TryParse(value, out intValue);
            _attractions = field switch
            {
                "Name" => _attractions.Where(s => s.Name == value),
                "Photo" => _attractions.Where(s => s.Photo == value),
                "AdmArea" => _attractions.Where(s => s.AdmArea == value),
                "District" => _attractions.Where(s => s.District == value),
                "Location" => _attractions.Where(s => s.Location == value),
                "RegistrationNumber" => _attractions.Where(s => s.RegistrationNumber == value),
                "State" => _attractions.Where(s => s.State == value),
                "LocationType" => _attractions.Where(s => s.LocationType == value),
                "global_id" => _attractions.Where(s => s.GlobalId == intValue),
                _ => _attractions
            };
        }
        return _attractions;
    }
}
