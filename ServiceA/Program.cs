using ConsulServerCore.ConsulServiceRegistration;
using ConsulServiceRegistration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddConsul();
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddControllersAsServices();

var app = builder.Build();
IOptions<ConsulServiceOptions> consulOptions=app.Services.GetRequiredService<IOptions<ConsulServiceOptions>>();
app.UseConsul();
app.UseHealthChecks(consulOptions.Value.HealthCheck);
app.UseAuthorization();

app.MapControllers();

app.Run();
