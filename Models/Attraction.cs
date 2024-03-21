using CsvHelper.Configuration.Attributes;
using System.Text.Json.Serialization;

namespace Models;

/// <summary>
/// Represents an attraction entity.
/// </summary>
[Delimiter(";")]
[Quote('"')]
public class Attraction
{
    private string _name;
    private string _photo;
    private string _admArea;
    private string _district;
    private string _location;
    private string _registrationNumber;
    private string _state;
    private string _locationType;
    private long _globalId;
    private string _geodataCenter;
    private string _geoarea;

    public Attraction(
        [Name("Name")] string name,
        [Name("Photo")] string photo,
        [Name("AdmArea")] string admArea,
        [Name("District")] string district,
        [Name("Location")] string location,
        [Name("RegistrationNumber")] string registrationNumber,
        [Name("State")] string state,
        [Name("LocationType")] string locationType,
        [Name("global_id")] long globalId,
        [Name("geodata_center")]string geodataCenter,
        [Name("geoarea")] string geoarea)
    {
        _name = name;
        _photo = photo;
        _admArea = admArea;
        _district = district;
        _location = location;
        _registrationNumber = registrationNumber;
        _state = state;
        _locationType = locationType;
        _globalId = globalId;
        _geodataCenter = geodataCenter;
        _geoarea = geoarea;
    }

    /// <summary>
    /// Gets or sets the name of the attraction.
    /// </summary>
    [Name("Name")]
    [JsonPropertyName("Name")]
    public string Name { get { return _name; } set { _name = value; }  }

    /// <summary>
    /// Gets or sets the photo URL of the attraction.
    /// </summary>
    [Name("Photo")]
    [JsonPropertyName("Photo")]
    public string Photo { get { return _photo; } set { _photo = value; } }

    /// <summary>
    /// Gets or sets the administrative area of the attraction.
    /// </summary>
    [Name("AdmArea")]
    [JsonPropertyName("AdmArea")]
    public string AdmArea { get { return _admArea; } set{ _admArea = value; } }

    /// <summary>
    /// Gets or sets the district of the attraction.
    /// </summary>
    [Name("District")]
    [JsonPropertyName("District")]
    public string District { get { return _district; } set{ _district = value; } }

    /// <summary>
    /// Gets or sets the location of the attraction.
    /// </summary>
    [Name("Location")]
    [JsonPropertyName("Location")]
    public string Location { get { return _location; } set { _location = value; } }

    /// <summary>
    /// Gets or sets the registration number of the attraction.
    /// </summary>
    [Name("RegistrationNumber")]
    [JsonPropertyName("RegistrationNumber")]
    public string RegistrationNumber { get { return _registrationNumber; } set { _registrationNumber = value; } }

    /// <summary>
    /// Gets or sets the state of the attraction.
    /// </summary>
    [Name("State")]
    [JsonPropertyName("State")]
    public string State { get { return _state; } set { _state = value; } }

    /// <summary>
    /// Gets or sets the type of location of the attraction.
    /// </summary>
    [Name("LocationType")]
    [JsonPropertyName("LocationType")]
    public string LocationType { get {  return _locationType; } init { _locationType = value; } }

    /// <summary>
    /// Gets or sets the global ID of the attraction.
    /// </summary>
    [Name("global_id")]
    [JsonPropertyName("global_id")]
    public long GlobalId { get { return _globalId; } init { _globalId = value;} }

    /// <summary>
    /// Gets or sets the geodata center of the attraction.
    /// </summary>
    [Name("geodata_center"), Optional]
    [JsonPropertyName("geodata_center")]
    public string GeodataCenter { get { return _geodataCenter; } set { _geodataCenter = value; } }

    /// <summary>
    /// Gets or sets the geo area of the attraction.
    /// </summary>
    [Name("geoarea"), Optional]
    [JsonPropertyName("geoarea")]
    public string GeoArea {  get { return _geoarea; } set { _geoarea = value; } }
}