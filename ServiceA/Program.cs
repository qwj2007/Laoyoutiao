using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using ConsulServerCore.ConsulServiceRegistration;
using ConsulServiceRegistration;
using Microsoft.Extensions.Options;
using PollyServerCore.PollyService;
using System.Reflection;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Add services to the container.
builder.Services.AddConsul();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Assembly asm = Assembly.GetExecutingAssembly();
foreach (var type in asm.GetExportedTypes())
{
    bool hasHystrixCommand = type.GetMethods().Any(m =>
        m.GetCustomAttribute(typeof(HystrixCommandAttribute)) != null);
    if (hasHystrixCommand)
    {
        builder.Services.AddSingleton(type);
       
    }
}
//
builder.Services.ConfigureDynamicProxy(config =>
{
    config.Interceptors.AddTyped<HystrixCommandAttribute>();
    //config.Interceptors.AddTyped<HystrixCommandAttribute>();
});


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
