
using Flight2.Domain.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;
using Flight2.MongoDB;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));




// Call the SeedFlights method on the mongoDbService instance

// Retrieve the MongoDBSettings from configuration
var mongoDBSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();

  
// Configure the MongoDB client and database
var mongoClient = new MongoClient(mongoDBSettings.ConnectionURI);
var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);

// Register the MongoDB service with the configured client and database
//builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
Console.WriteLine("DatabaseName: " + mongoDatabase.DatabaseNamespace.DatabaseName);

builder.Services.AddSingleton<MongoDBService>(sp =>
{
    // Get the MongoDBSettings from the configuration
    var mongoDBSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    // Create and return a new MongoDBService instance
    return new MongoDBService(Options.Create(mongoDBSettings));
});

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c =>
{
    c.DescribeAllParametersInCamelCase();
    c.AddServer(new OpenApiServer
    {
        Description = "Developpement Server",
        Url = "https://localhost:7073"
    });
    c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"] + e.ActionDescriptor.RouteValues["controller"]}");
});



var app = builder.Build();

var random = new Random();





// Insert the flightsToSeed data into the collection


app.UseCors(builder => builder.WithOrigins("*")
.AllowAnyMethod()
.AllowAnyHeader());
app.UseSwagger().UseSwaggerUI();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();


