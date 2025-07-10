using Autofac;

using Laoyoutiao.Configuration;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Tasks.Core;
using Laoyoutiao.webapi.Filter;
using Laoyoutiao.webapi.Util;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//注册服务
builder.ServiceRegister();

var app = builder.Build();
app.UseAppRegister();



