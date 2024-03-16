namespace DataManager.Mapping;

public class DataField : Enumeration
{
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

    public DataField(int id, string name) : base(id, name) { }

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
        };
}
