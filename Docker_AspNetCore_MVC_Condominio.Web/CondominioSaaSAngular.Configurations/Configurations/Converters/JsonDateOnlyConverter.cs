using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CondominioSaaSAngular.Configurations.Converters;

public class JsonDateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var value = reader.GetString();
            if (DateOnly.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                return DateOnly.FromDateTime(dateTime);

            throw new JsonException($"Formato de data inválido. Esperado: {Format}");
        }
        catch (Exception ex)
        {
            throw new JsonException($"Erro ao converter data: {ex.Message}");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}