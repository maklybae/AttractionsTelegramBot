using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Models.DataFormatProcessors;

/// <summary>
/// Provides functionality to read from and write to CSV files using CsvHelper library.
/// </summary>
public class CSVProcessing
{
    /// <summary>
    /// Initializes a new instance of the CSVProcessing class.
    /// </summary>
    public CSVProcessing() { }

    private readonly static CsvConfiguration s_config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        Quote = '"',
    };

    /// <summary>
    /// Writes the second header row for CSV files.
    /// </summary>
    /// <param name="csv">The CsvWriter instance to write to.</param>
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

    /// <summary>
    /// Reads CSV data from the input stream and returns a list of Attraction records.
    /// </summary>
    /// <param name="inputStream">The input stream containing CSV data.</param>
    /// <returns>A list of Attraction records read from the CSV data.</returns>
    public List<Attraction> Read(Stream inputStream)
    {
        inputStream.Position = 0;
        using var reader = new StreamReader(inputStream);

        using var csv = new CsvReader(reader, s_config);

        csv.Read();
        if (!csv.ReadHeader())
            throw new ReaderException(new CsvContext(csv));
        csv.Read();

        return csv.GetRecords<Attraction>().ToList();
    }

    /// <summary>
    /// Writes the provided collection of Attraction records to a CSV format in a MemoryStream and returns the stream.
    /// </summary>
    /// <param name="records">The collection of Attraction records to write to CSV.</param>
    /// <returns>A MemoryStream containing the CSV data.</returns>
    public MemoryStream Write(IEnumerable<Attraction> records)
    {
        var stream = new MemoryStream();

        var writer = new StreamWriter(stream, leaveOpen: true);
        var csv = new CsvWriter(writer, s_config, leaveOpen: true);

        csv.WriteHeader<Attraction>();
        WriteSecondHeader(csv);
        csv.WriteRecords(records);

        writer.Dispose();
        csv.Dispose();

        stream.Position = 0;
        return stream;
    }
}
