using System.Text.Encodings.Web;
using System.Text.Json;

namespace Models.DataFormatProcessors;

public class JSONProcessing
{
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public JSONProcessing() { }

    public MemoryStream Write(IEnumerable<Attraction> records)
    {
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, records, s_serializerOptions);
        stream.Position = 0;
        return stream;
    }

    public List<Attraction> Read(Stream inputStream)
    {
        inputStream.Position = 0;
        return JsonSerializer.Deserialize<List<Attraction>>(inputStream, s_serializerOptions) ?? new List<Attraction>();
    }

}
