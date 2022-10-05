using MongoDbMiniApi;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDb"));
//services.AddSingleton(new MongoClient(builder.Configuration.GetSection("MongoDB").GetValue<string>("ConnectionString")));
services.AddSingleton<IMongoRepository, MongoRepository>();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("AddBeer", (IMongoRepository mongoRepository) =>
{
    var beer = new Beer("HopTopBeer", 5.6, "Ale");

    return Results.Ok(mongoRepository.AddBeerToCollectionAsync(beer));
});

app.MapGet("GetBeers", (IMongoRepository mongoRepository) =>
{
    var beer = mongoRepository.GetBeersFromCollection();
    return Results.Ok(beer);
});

app.MapGet("GetBeersAsync", async (IMongoRepository mongoRepository) =>
{
    var beer = await mongoRepository.GetBeersFromCollectionAsync();
    return Results.Ok(beer);
});

app.MapGet("/GetDatabasesAsyncMaybe", async (IMongoRepository mongoRepository) =>
    Results.Ok(await mongoRepository.GetDatabases()))
.WithName("GetDatabases");

app.MapGet("/GetDatabaseNames", (IMongoRepository mongoRepository) =>
    Results.Ok(mongoRepository.GetDatabaseNames()))
.WithName("GetDatabaseNames");

app.Run();