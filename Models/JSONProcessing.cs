using System.Text.Encodings.Web;
using System.Text.Json;

namespace Models;

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
        return stream;
    }

    public List<Attraction> Read(Stream inputStream)
    {
        return JsonSerializer.Deserialize<List<Attraction>>(inputStream, s_serializerOptions);
    }

}
