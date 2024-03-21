using System.Text.Encodings.Web;
using System.Text.Json;

namespace Models.DataFormatProcessors;

/// <summary>
/// Provides functionality to process JSON data using System.Text.Json library.
/// </summary>
public class JSONProcessing
{
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Initializes a new instance of the JSONProcessing class.
    /// </summary>
    public JSONProcessing() { }

    /// <summary>
    /// Writes the provided collection of Attraction records to a JSON format in a MemoryStream and returns the stream.
    /// </summary>
    /// <param name="records">The collection of Attraction records to write to JSON.</param>
    /// <returns>A MemoryStream containing the JSON data.</returns>
    public MemoryStream Write(IEnumerable<Attraction> records)
    {
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, records, s_serializerOptions);
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// Reads JSON data from the input stream and returns a list of Attraction records.
    /// </summary>
    /// <param name="inputStream">The input stream containing JSON data.</param>
    /// <returns>A list of Attraction records read from the JSON data.</returns>
    public List<Attraction> Read(Stream inputStream)
    {
        inputStream.Position = 0;
        return JsonSerializer.Deserialize<List<Attraction>>(inputStream, s_serializerOptions) ?? new List<Attraction>();
    }

}
