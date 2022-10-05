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

app.MapGet("/GetDatabesesAsync", async (IMongoRepository mongoRepository) =>
{
    return await mongoRepository.GetDatabases();
})
.WithName("GetDatabases");

app.MapGet("/GetDatabeseNames", (IMongoRepository mongoRepository) =>
{
    return mongoRepository.GetDatabaseNames();
})
.WithName("GetDatabeseNames");

app.Run();