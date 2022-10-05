using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbMiniApi;
public interface IMongoRepository
{
    Task<IReadOnlyList<BsonDocument>> GetDatabases();
    IReadOnlyList<string> GetDatabaseNames();
}

public class MongoRepository : IMongoRepository
{
    private readonly MongoClient _mongoClient;
    private readonly MongoDbOptions _mongoDbOptions;
    public MongoRepository(IOptions<MongoDbOptions> options)
    {
        _mongoDbOptions = options.Value;
        _mongoClient = new MongoClient(_mongoDbOptions.ConnectionString);
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