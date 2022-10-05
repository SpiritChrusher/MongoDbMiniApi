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
services.AddSingleton<IMongoRepository, MongoRepository>();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("AddBeer", (IMongoRepository mongoRepository, [FromBody]Beer beerRequest) =>
{
    mongoRepository.AddBeerToCollectionAsync(beerRequest);
    return Results.Ok();
});

app.MapGet("GetBeersAsync", async (IMongoRepository mongoRepository) =>
    Results.Ok(await mongoRepository.GetBeersFromCollectionAsync()));

app.MapPost("UpdateBeer", async(IMongoRepository mongoRepository, [FromBody]Beer updatedBeer) =>
{
    await mongoRepository.UpdateBeer(updatedBeer);
    return Results.Ok();
});

app.MapDelete("/DeleteDatabaseNames/{id}", async (IMongoRepository mongoRepository, [FromRoute] string id) =>
{
    await mongoRepository.DeleteBeer(id);
    Results.NoContent();
});

app.Run();