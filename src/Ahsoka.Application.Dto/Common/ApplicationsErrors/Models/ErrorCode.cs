using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
public record ErrorCode
{
    protected ErrorCode(int value) { Value = value; }

    public int Value { get; init; }

    public static ErrorCode New(int value) => new(value);

    public override string ToString() => Value.ToString();

    public int CompareTo(ErrorCode? other) => Value.CompareTo(other?.Value);

    public static implicit operator ErrorCode(int value) => new(value);

    public static implicit operator int(ErrorCode accountId) => accountId.Value;
}

public class ErrorCodeConverter : JsonConverter<ErrorCode>
{
    public override ErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                int value = doc.RootElement.GetProperty("value").GetInt32();
                return ErrorCode.New(value);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return ErrorCode.New(reader.GetInt32());
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ErrorCode value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}