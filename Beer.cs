using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbMiniApi;

public record BeerPoco(
    int Id,
    string Name,
    double Alcohol,
    string Type);

public class Beer
{
    [BsonId]
    public string Name { get; set; }
    [BsonElement("alcohol")]
    public double Alcohol { get; set; }
    [BsonElement("type")]
    public string Type { get; set; }

    public Beer(string name, double alcohol, string type)
    {
        Name=name;
        Alcohol=alcohol;
        Type=type;
    }
}
