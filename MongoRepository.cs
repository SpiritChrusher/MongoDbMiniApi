using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbMiniApi;
public interface IMongoRepository
{
    Task AddBeerToCollectionAsync(Beer beer);
    Task<List<string>> GetBeersFromCollectionAsync();
    BsonDocument GetBeersFromCollection();
    Task<IReadOnlyList<BsonDocument>> GetDatabases();
    IReadOnlyList<string> GetDatabaseNames();
}

public class MongoRepository : IMongoRepository
{
    private readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private readonly MongoDbOptions _mongoDbOptions;
    public MongoRepository(IOptions<MongoDbOptions> options)
    {
        _mongoDbOptions = options.Value;
        _mongoClient = new MongoClient(_mongoDbOptions.ConnectionString);
        _database = _mongoClient.GetDatabase(_mongoDbOptions.Database);
    }

    public async Task AddBeerToCollectionAsync(Beer beer)
    {
        //var bsonBeers = beer.ToBsonDocument();
        var document = new BsonDocument
            {
                { "student_id", 10000 },
                { "scores", new BsonArray
                    {
                    new BsonDocument{ {"type", "exam"}, {"score", 88.12334193287023 } },
                    new BsonDocument{ {"type", "quiz"}, {"score", 74.92381029342834 } }
                    }
                },
                { "class_id", 480}
            };

        var collection = _database.GetCollection<BsonDocument>("Beers");
        try
        {
            await collection.InsertOneAsync(document);

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<string>> GetBeersFromCollectionAsync()
    {
        var collection = _database.GetCollection<BsonDocument>("Beers");
        try
        {
            var asyncCursor = await collection.FindAsync(new BsonDocument());
            var dbList = new List<string>();
            while (await asyncCursor.MoveNextAsync())
            {
                foreach (var name in asyncCursor.Current)
                {
                    dbList.Add(name.ToString());
                }
            }

            return dbList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public BsonDocument GetBeersFromCollection()
    {
        var collection = _database.GetCollection<BsonDocument>("Beers");
        try
        {
            var document = collection.Find(new BsonDocument()).FirstOrDefault();
            
            return document;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IReadOnlyList<BsonDocument>> GetDatabases()
    {
        using var asyncCursor = await _mongoClient.ListDatabasesAsync();

        var dbList = new List<BsonDocument>();
        while (await asyncCursor.MoveNextAsync())
        {
            foreach (var name in asyncCursor.Current)
            {
                dbList.Add(name);
            }
        }

        return dbList;
    }

    public IReadOnlyList<string> GetDatabaseNames()
    {
        var dbList = _mongoClient.ListDatabaseNames().ToList(); 

        Console.WriteLine("The list of databases on this server is: ");
        foreach (var db in dbList)
        {
            Console.WriteLine(db);
        }
        return dbList;
    }
}