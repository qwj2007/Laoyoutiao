using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json").Build();
//builder.Services.AddOcelot(config)..AddConsul().AddPolly();//��polly�۶�
// Add services to the container.
builder.Services.AddOcelot().AddConsul().AddPolly();
builder.Services.AddLogging(builder =>
{
    builder.AddFilter("Ocelot", LogLevel.Warning);
    //builder.AddLog4Net();
});
var app = builder.Build();
app.UseOcelot().Wait();
// Configure the HTTP request pipeline.

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//});

app.Run();

//internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
