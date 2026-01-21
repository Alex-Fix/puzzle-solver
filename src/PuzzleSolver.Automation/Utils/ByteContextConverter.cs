using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuzzleSolver.Automation.Utils;

internal class ByteContextConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray().Select(x => x.GetByte()).ToArray();

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options) 
        => JsonSerializer.Serialize(writer, value, options);
}
