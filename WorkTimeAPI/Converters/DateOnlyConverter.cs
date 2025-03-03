using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DateOnlyConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd"; // Формат без времени

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
