using ConsulServerCore.ConsulServiceRegistration;
using ConsulServiceRegistration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddConsul();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
IOptions<ConsulServiceOptions> consulOptions=app.Services.GetRequiredService<IOptions<ConsulServiceOptions>>();
app.UseConsul();
app.UseHealthChecks(consulOptions.Value.HealthCheck);
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.Run();
