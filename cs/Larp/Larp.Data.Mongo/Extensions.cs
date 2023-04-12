using System.Linq.Expressions;
using MongoDB.Driver;

namespace Larp.Data.Mongo;

public static class MongoCollectionExtensions
{
    public static async Task<TDocument?> FindOneAsync<TDocument>(this IMongoCollection<TDocument> collection,
        Expression<Func<TDocument, bool>> filter, FindOptions? options = null) =>
        await collection.Find(filter, options).FirstOrDefaultAsync();

    public static async Task<UpdateResult> UpdateByIdAsync<TDocument>(this IMongoCollection<TDocument> collection,
        Expression<Func<TDocument, bool>> filter,
        Func<UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> update)
    {
        return await collection.UpdateOneAsync(filter, update(Builders<TDocument>.Update));
    }

    public static async Task<TDocument[]> ToArrayAsync<TDocument>(this IAsyncCursorSource<TDocument> source,
        CancellationToken cancellationToken = default)
    {
        using var cursor = await source.ToCursorAsync(cancellationToken).ConfigureAwait(false);
        var list = await cursor.ToListAsync(cancellationToken).ConfigureAwait(false);
        return list.ToArray();
    }
}