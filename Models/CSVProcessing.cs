using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Models;

public class CSVProcessing
{
    public IEnumerable<Attraction> Read(Stream inputStream)
    {
        using var reader = new StreamReader(inputStream);
        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Quote = '"',
        };
        using var csv = new CsvReader(reader, config);

        csv.Read();
        if (!csv.ReadHeader())
            throw new ReaderException(new CsvContext(csv));
        csv.Read();
        // TODO: Validation of header rows

        return csv.GetRecords<Attraction>();
    }
}
