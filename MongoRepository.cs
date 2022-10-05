using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbMiniApi;
public interface IMongoRepository
{
    Task AddBeerToCollectionAsync(Beer beer);
    Task<List<Beer>> GetBeersFromCollectionAsync();
    Task<Beer> GetBeerByIdFromCollectionAsync(string id);
    Task UpdateBeer(Beer updatedBeer);
    Task DeleteBeer(string id);
}

public class MongoRepository : IMongoRepository
{
    private IMongoDatabase _database;
    private readonly MongoDbOptions _mongoDbOptions;
    private IMongoCollection<Beer> _mongoCollection;
    public MongoRepository(IOptions<MongoDbOptions> options, IMongoClient mongoClient)
    {
        _mongoDbOptions = options.Value;
        _database = mongoClient.GetDatabase(_mongoDbOptions.Database);
        _mongoCollection = _database.GetCollection<Beer>(_mongoDbOptions.BeerCollectionName);
    }

    public async Task AddBeerToCollectionAsync(Beer beer)
    {
        try
        {
            await _mongoCollection.InsertOneAsync(beer);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<Beer>> GetBeersFromCollectionAsync()
    {
        try
        {
            var asyncCursor = await _mongoCollection.FindAsync(new BsonDocument());
            var dbList = new List<Beer>();
            while (await asyncCursor.MoveNextAsync())
            {
                foreach (var beer in asyncCursor.Current)
                {
                    dbList.Add(beer);
                }
            }

            return dbList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Beer> GetBeerByIdFromCollectionAsync(string id)
    {
        try
        {
            var beer = await _mongoCollection.Find(b => b.Name == id).FirstOrDefaultAsync();
            return beer;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateBeer(Beer updatedBeer)
    {
        try
        {
            await _mongoCollection.ReplaceOneAsync(b => b.Name == updatedBeer.Name, updatedBeer);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task DeleteBeer(string id)
    {
        try
        {
            await _mongoCollection.DeleteOneAsync(b => b.Name == id);
        }
        catch (Exception)
        {

            throw;
        }
    }

    private List<Beer> GetBeersFromCollection()
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
