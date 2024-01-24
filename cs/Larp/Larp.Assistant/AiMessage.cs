namespace Larp.Assistant;

public record AiMessage(string Id, bool Input, string Role, string Content, DateTime CreatedAt);