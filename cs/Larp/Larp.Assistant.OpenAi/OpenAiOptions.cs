namespace Larp.Assistant.OpenAi;

public class OpenAiOptions
{
    public const string SectionName = "OpenAi";
    public string? ApiKey { get; set; }
    public string? OrganizationId { get; set; }
    public string? GameMasterAssistantId { get; set; }
    public string? PlayerAssistantId { get; set; }
    public string Endpoint { get; set; } = "https://api.openai.com";
    public string? ApplicationName { get; set; } = "Mystwood Tavern";
}