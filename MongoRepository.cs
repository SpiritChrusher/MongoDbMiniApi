using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbMiniApi;
public interface IMongoRepository<T>
{
    Task AddOneToCollectionAsync(T element);
    Task<List<T>> GetFromCollectionAsync();
    Task<T> GetByIdFromCollectionAsync(string id);
    Task UpdateOneAsync(T updatedElement, string id);
    Task DeleteOneAsync(string id);
    Task DeleteAllAsync();
}

public class MongoRepository<T> : IMongoRepository<T>
{
    private IMongoDatabase _database;
    private readonly MongoDbOptions _mongoDbOptions;
    private IMongoCollection<T> _mongoCollection;
    public MongoRepository(IOptions<MongoDbOptions> options, IMongoClient mongoClient)
    {
        _mongoDbOptions = options.Value;
        _database = mongoClient.GetDatabase(_mongoDbOptions.Database);
        _mongoCollection = _database.GetCollection<T>($"{nameof(T)}s");
    }

    public async Task AddOneToCollectionAsync(T element)
    {
        try
        {
            await _mongoCollection.InsertOneAsync(element);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<T>> GetFromCollectionAsync()
    {
        try
        {
            var asyncCursor = await _mongoCollection.FindAsync(new BsonDocument());
            var dbList = new List<T>();
            while (await asyncCursor.MoveNextAsync())
            {
                foreach (var element in asyncCursor.Current)
                {
                    dbList.Add(element);
                }
            }

            return dbList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<T> GetByIdFromCollectionAsync(string id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var element = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            return element;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateOneAsync(T updatedElement, string id)
    {
        try
        {
            var result = await _mongoCollection.ReplaceOneAsync(new BsonDocument("_id", id),
                updatedElement,
                new UpdateOptions { IsUpsert = true});
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task DeleteOneAsync(string id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _mongoCollection.DeleteOneAsync(filter);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task DeleteAllAsync()
    {
        try
        {
            var filter = Builders<T>.Filter.AnyGte("_id", "");
            await _mongoCollection.DeleteManyAsync(filter);
        }
        catch (Exception)
        {

            throw;
        }
    }

    private List<T> GetBeersFromCollection()
    {
        try
        {
            var document = _mongoCollection.Find(new BsonDocument()).ToList();
            
            return document;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
