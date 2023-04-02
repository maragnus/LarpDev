using System.Linq.Expressions;
using MongoDB.Driver;

namespace Larp.Data.Mongo;

public static class MongoCollectionExtensions
{
    public static async Task<TDocument?> FindOneAsync<TDocument>(this IMongoCollection<TDocument> collection,
        Expression<Func<TDocument, bool>> filter, FindOptions? options = null) =>
         await collection.Find(filter, options).FirstOrDefaultAsync();

    public static async Task<UpdateResult> UpdateByIdAsync<TDocument>(this IMongoCollection<TDocument> collection, Expression<Func<TDocument, bool>> filter, Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> update)
    {
        return await collection.UpdateOneAsync(filter, update(Builders<TDocument>.Update));
    }

}