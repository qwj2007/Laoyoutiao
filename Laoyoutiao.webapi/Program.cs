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
app.UseRouting();
#region 鉴权
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region 跨域设置
app.UseCors("CorsPolicy");
#endregion


//app.UseEndpoints(routes =>
//{
//    routes.MapControllerRoute(
//        name: "TurntableRoute",
//        pattern: "{area:exists}/{controller=Activity}/{action=Turntable}/{id}.html");

//    routes.MapControllerRoute(
//        name: "areaRoute",
//        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//    routes.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
//app.MapControllers();

app.Run();
