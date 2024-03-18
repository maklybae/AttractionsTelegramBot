namespace Models;

public class Sorter
{
    private IEnumerable<Attraction> _attractions;
    private IEnumerable<(string field, bool isDescending)> _sortingParams;

    public Sorter(IEnumerable<Attraction> attractions, IEnumerable<(string field, bool isDesecnding)> sortingParams)
    {
        _attractions = attractions;
        _sortingParams = sortingParams;
    }

    public IEnumerable<Attraction> Sort()
    {
        var enumerator = _sortingParams.GetEnumerator();
        enumerator.MoveNext();
        var orderer = PrimarySorterByAlternativeName(enumerator.Current.field, enumerator.Current.isDescending);

        while (enumerator.MoveNext())
        {
            orderer = SecondarySorterByAlternativeName(orderer, enumerator.Current.field, enumerator.Current.isDescending);
        }
        return orderer ?? _attractions;
    }


    private IOrderedEnumerable<Attraction>? PrimarySorterByAlternativeName(string title, bool isDescending) =>
        title switch
        {
            "Name" => isDescending ? _attractions.OrderByDescending(s => s.Name) : _attractions.OrderBy(s => s.Name),
            "Photo" => isDescending ? _attractions.OrderByDescending(s => s.Photo) : _attractions.OrderBy(s => s.Photo),
            "AdmArea" => isDescending ? _attractions.OrderByDescending(s => s.AdmArea) : _attractions.OrderBy(s => s.AdmArea),
            "District" => isDescending ? _attractions.OrderByDescending(s => s.District) : _attractions.OrderBy(s => s.District),
            "Location" => isDescending ? _attractions.OrderByDescending(s => s.Location) : _attractions.OrderBy(s => s.Location),
            "RegistrationNumber" => isDescending ? _attractions.OrderByDescending(s => s.RegistrationNumber) : _attractions.OrderBy(s => s.RegistrationNumber),
            "State" => isDescending ? _attractions.OrderByDescending(s => s.State) : _attractions.OrderBy(s => s.State),
            "LocationType" => isDescending ? _attractions.OrderByDescending(s => s.LocationType) : _attractions.OrderBy(s => s.LocationType),
            "global_id" => isDescending ? _attractions.OrderByDescending(s => s.GlobalId) : _attractions.OrderBy(s => s.GlobalId),
            _ => isDescending ? _attractions.OrderByDescending(s => s.Name) : _attractions.OrderBy(s => s.Name),
        };

    private IOrderedEnumerable<Attraction>? SecondarySorterByAlternativeName(IOrderedEnumerable<Attraction>? orderer, string title, bool isDescending) =>
        title switch
            {
                "Name" => isDescending? orderer?.ThenByDescending(s => s.Name) : orderer?.ThenBy(s => s.Name),
                "Photo" => isDescending? orderer?.ThenByDescending(s => s.Photo) : orderer?.ThenBy(s => s.Photo),
                "AdmArea" => isDescending? orderer?.ThenByDescending(s => s.AdmArea) : orderer?.ThenBy(s => s.AdmArea),
                "District" => isDescending? orderer?.ThenByDescending(s => s.District) : orderer?.ThenBy(s => s.District),
                "Location" => isDescending? orderer?.ThenByDescending(s => s.Location) : orderer?.ThenBy(s => s.Location),
                "RegistrationNumber" => isDescending? orderer?.ThenByDescending(s => s.RegistrationNumber) : orderer?.ThenBy(s => s.RegistrationNumber),
                "State" => isDescending? orderer?.ThenByDescending(s => s.State) : orderer?.ThenBy(s => s.State),
                "LocationType" => isDescending? orderer?.ThenByDescending(s => s.LocationType) : orderer?.ThenBy(s => s.LocationType),
                "global_id" => isDescending? orderer?.ThenByDescending(s => s.GlobalId) : orderer?.ThenBy(s => s.GlobalId),
                _ => isDescending? orderer?.ThenByDescending(s => s.Name) : orderer?.ThenBy(s => s.Name)
            };
}
