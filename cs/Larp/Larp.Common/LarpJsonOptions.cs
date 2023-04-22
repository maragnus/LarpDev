using System.Text.Json;
using System.Text.Json.Serialization;

namespace Larp.Common;

public static class LarpJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  
    };
}