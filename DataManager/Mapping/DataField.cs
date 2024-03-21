namespace DataManager.Mapping;

/// <summary>
/// Represents a specific type of enumeration for data fields.
/// </summary>
public class DataField : Enumeration
{
    // Below are data fieds that stored in sample file.
    public static DataField Name = new(0, "Name");
    public static DataField Photo = new(1, "Photo");
    public static DataField AdmArea = new(2, "AdmArea");
    public static DataField District = new(3, "District");
    public static DataField Location = new(4, "Location");
    public static DataField RegistrationNumber = new(5, "RegistrationNumber");
    public static DataField State = new(6, "State");
    public static DataField LocationType = new(7, "LocationType");
    public static DataField GlobalId = new(8, "global_id");
    public static DataField GeodataCenter = new(9, "geodata_center");
    public static DataField Geoarea = new(10, "geoarea");

    /// <summary>
    /// Initializes a new instance of the DataField class with the specified ID and name.
    /// </summary>
    /// <param name="id">The ID of the data field.</param>
    /// <param name="name">The name of the data field.</param>
    public DataField(int id, string name) : base(id, name) { }

    /// <summary>
    /// Retrieves the DataField instance based on the provided index.
    /// </summary>
    /// <param name="index">The index of the data field to retrieve.</param>
    /// <returns>The DataField instance corresponding to the index, or the 'Name' data field if the index is out of range.</returns>
    public static DataField GetDataField(int index) =>
        index switch
        {
            0 => Name,
            1 => Photo,
            2 => AdmArea,
            3 => District,
            4 => Location,
            5 => RegistrationNumber,
            6 => State,
            7 => LocationType,
            8 => GlobalId,
            9 => GeodataCenter,
            10 => Geoarea,
            _ => Name
            // Default to Name if the index is out of range.
        };
}
