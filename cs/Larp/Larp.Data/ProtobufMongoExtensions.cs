using Google.Protobuf;
using MongoDB.Bson;

namespace Larp.Data;

public static class ProtobufMongoExtensions
{
    public static TMessage ToMessage<TMessage>(this BsonDocument document)
        where TMessage : IMessage, new()
    {
        return JsonParser.Default.Parse<TMessage>(document.ToJson());
    }
    public static BsonDocument ToBsonDocument(this IMessage message)
    {
        var json = JsonFormatter.Default.Format(message);
        return BsonDocument.Parse(json);
    }
}