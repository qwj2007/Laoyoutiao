using Autofac;
using Laoyoutiao.Configuration;
using Laoyoutiao.IService;
using Laoyoutiao.Service;
using Laoyoutiao.webapi.Filter;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<MvcOptions>(opt => { opt.Filters.Add<SysExceptionFilter>(); });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Register();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region 鉴权
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region 跨域设置
app.UseCors("CorsPolicy");
#endregion

app.MapControllers();

app.Run();
