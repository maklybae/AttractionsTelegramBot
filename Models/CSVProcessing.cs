using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Models;

public class CSVProcessing
{
    public CSVProcessing() { }

    private readonly static CsvConfiguration s_config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        Quote = '"',
    };

    private void WriteSecondHeader(CsvWriter csv)
    {
        csv.NextRecord();
        csv.WriteField("Название объекта");
        csv.WriteField("Фотография");
        csv.WriteField("Административный округ по адресу");
        csv.WriteField("Район");
        csv.WriteField("Месторасположение");
        csv.WriteField("Государственный регистрационный знак");
        csv.WriteField("Состояние регистрации");
        csv.WriteField("Тип места расположения");
        csv.WriteField("global_id");
        csv.WriteField("geodata_center");
        csv.WriteField("geoarea");
        csv.NextRecord();
    }

    public List<Attraction> Read(Stream inputStream)
    {
        using var reader = new StreamReader(inputStream);
        
        using var csv = new CsvReader(reader, s_config);

        csv.Read();
        if (!csv.ReadHeader())
            throw new ReaderException(new CsvContext(csv));
        csv.Read();
        // TODO: Validation of header rows

        return csv.GetRecords<Attraction>().ToList();
    }

    public MemoryStream Write(IEnumerable<Attraction> records)
    {
        // TODO: Close stream
        var stream = new MemoryStream();
        
        var writer = new StreamWriter(stream, leaveOpen: true);
        var csv = new CsvWriter(writer, s_config, leaveOpen: true);

        csv.WriteHeader<Attraction>();
        WriteSecondHeader(csv);
        csv.WriteRecords(records);

        writer.Dispose();
        csv.Dispose();

        return stream;
    }
}
