﻿using CsvHelper.Configuration.Attributes;

namespace Models;

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


    [Name("Name")]
    public string Name { get { return _name; } set { _name = value; }  }

    [Name("Photo")]
    public string Photo { get { return _photo; } set { _photo = value; } }

    [Name("AdmArea")]
    public string AdmArea { get { return _admArea; } set{ _admArea = value; } }

    [Name("District")]
    public string District { get { return _district; } set{ _district = value; } }

    [Name("Location")]
    public string Location { get { return _location; } set { _location = value; } }

    [Name("RegistrationNumber")]
    public string RegistrationNumber { get { return _registrationNumber; } set { _registrationNumber = value; } }

    [Name("State")]
    public string State { get { return _state; } set { _state = State; } }

    [Name("LocationType")]
    public string LocataionType { get {  return _locationType; } init { _locationType = value; } }

    [Name("global_id")]
    public long GlobalId { get { return _globalId; } init { _globalId = value;} }

    [Name("geodata_center"), Optional]
    public string GeodataCenter { get { return _geodataCenter; } set { _geodataCenter = value; } }

    [Name("geoarea"), Optional]
    public string GeoArea {  get { return _geoarea; } set { _geoarea = value; } }
}