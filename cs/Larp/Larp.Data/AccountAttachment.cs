using System.Text.Json.Serialization;

namespace Larp.Data;

public class AccountAttachment
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string AttachmentId { get; set; } = default!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = default!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string UploadedBy { get; set; } = default!;

    public DateTimeOffset UploadedOn { get; set; }
    
    public string Title { get; set; } = default!;
    
    public string? MediaType { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public byte[]? Data { get; set; }
    public string? FileName { get; set; }
    
    public string? ThumbnailMediaType { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public byte[]? ThumbnailData { get; set; }
    public string? ThumbnailFileName { get; set; }

}