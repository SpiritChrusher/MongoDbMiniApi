using MongoDbMiniApi;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDb"));
services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var url = builder.Configuration.GetSection("MongoDb").GetValue<string>("ConnectionString");
    return new MongoClient(url);
});
services.AddSingleton<IMongoRepository<Beer>, MongoRepository<Beer>>();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("AddBeer", (IMongoRepository<Beer> mongoRepository, [FromBody]Beer beerRequest) =>
{
    mongoRepository.AddOneToCollectionAsync(beerRequest);
    return Results.Ok();
});

app.MapGet("GetBeersAsync", async (IMongoRepository<Beer> mongoRepository) =>
    Results.Ok(await mongoRepository.GetFromCollectionAsync()));

app.MapGet("GetBeerAsync/{id}", async (IMongoRepository<Beer> mongoRepository, [FromRoute] string id) =>
    Results.Ok(await mongoRepository.GetByIdFromCollectionAsync(id)));

app.MapPost("UpdateBeer", async(IMongoRepository<Beer> mongoRepository, [FromBody]Beer updatedBeer) =>
{
    await mongoRepository.UpdateOneAsync(updatedBeer, updatedBeer.Name);
    return Results.Ok();
});

app.MapDelete("/DeleteBeer/{id}", async (IMongoRepository<Beer> mongoRepository, [FromRoute] string id) =>
{
    await mongoRepository.DeleteOneAsync(id);
    Results.NoContent();
});

app.MapDelete("/DeleteAllBeers", async (IMongoRepository<Beer> mongoRepository) =>
{
    await mongoRepository.DeleteAllAsync();
    Results.NoContent();
});

app.Run();