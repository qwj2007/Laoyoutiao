using Laoyoutiao.Configuration;

var builder = WebApplication.CreateBuilder(args);
//注册服务
builder.ServiceRegister();

var app = builder.Build();
app.UseAppRegister();



