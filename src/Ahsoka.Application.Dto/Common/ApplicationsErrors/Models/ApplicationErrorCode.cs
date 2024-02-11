using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
public record ApplicationErrorCode
{
    protected ApplicationErrorCode(int value) { Value = value; }

    public int Value { get; init; }

    public static ApplicationErrorCode New(int value) => new(value);

    public override string ToString() => Value.ToString();

    public int CompareTo(ApplicationErrorCode? other) => Value.CompareTo(other?.Value);

    public static implicit operator ApplicationErrorCode(int value) => new(value);

    public static implicit operator int(ApplicationErrorCode accountId) => accountId.Value;
}

public class ErrorCodeConverter : JsonConverter<ApplicationErrorCode>
{
    public override ApplicationErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                int value = doc.RootElement.GetProperty("value").GetInt32();
                return ApplicationErrorCode.New(value);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return ApplicationErrorCode.New(reader.GetInt32());
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ApplicationErrorCode value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}