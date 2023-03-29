using System.Text.Json;
using System.Text.Json.Serialization;

namespace Larp.Landing.Shared;

public static class LarpJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  
    };
}