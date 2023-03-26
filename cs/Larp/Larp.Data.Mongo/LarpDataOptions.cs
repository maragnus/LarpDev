namespace Larp.Data.Mongo;

public class LarpDataOptions
{
    public const string SectionName = "LarpData";
    public string? ConnectionString { get; set; }
    public string? Database { get; set; }
}