using Autofac;
using Autofac.Extensions.DependencyInjection;
using DotNetCore.CAP.Messages;
using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Tasks.Core;
using Laoyoutiao.Util;
using Laoyoutiao.webapi.Config;
using Laoyoutiao.webapi.Extensions;
using Laoyoutiao.webapi.Filter;
using Laoyoutiao.webapi.Util;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Quartz;
using Quartz.Simpl;
using Serilog;
using SqlSugar;
using SqlSugar.IOC;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text;



namespace Laoyoutiao.Configuration
{
    public static class HostBuilderExtend
    {
        public static void ServiceRegister(this WebApplicationBuilder buil)
        {
            // Add services to the container.
            buil.Services.AddDistributedMemoryCache();
            // 添加 Session 服务
            buil.Services.AddSession(options =>
            {
                options.Cookie.Name = "CaptchaSession";
                options.IdleTimeout = TimeSpan.FromMinutes(5); // 5分钟过期
            });
            buil.Services.AddControllers();
            buil.Services.Configure<MvcOptions>(opt =>
            {
                //opt.Filters.Add<SysExceptionFilter>();
                opt.Filters.Add<CustomerActionFilters>();//全局注册，所有方法都可以使用actionfilter
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            buil.Services.AddEndpointsApiExplorer();


            #region 日志配置
            SerilogConfig.CreateLogger();
            #endregion

            #region 添加Apollo配置中心
            buil.WebHost.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                //添加Apollo配置中心
                //configurationBuilder.AddApollo(hostBuilderContext.Configuration.GetSection("apollo"))
                //    .AddNamespace("ApolloServiceConfig", ConfigFileFormat.Json).AddDefault();
            });
            buil.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // buil.Host.ConfigureAppConfiguration((context, b) =>
            // {
            //     //添加Apollo配置中心
            //     b.AddApollo(b.Build().GetSection("apollo"))
            //         //b.AddApollo(configuration.GetSection("apollo"))
            //         .AddDefault();
            // });
            #endregion

            #region  添加MediatR事件总线
            buil.Services.AddMediatR(Assembly.GetExecutingAssembly());
            #endregion

            #region 配置数据库

            #region 注入数据库
            buil.Services.AddSingleton(new AppSettings(buil.Configuration));
            SqlsugarSetup.AddSqlsugarSetup();
            SnowFlakeSingle.WorkId = Convert.ToInt32(buil.Configuration.GetSection("SnowFlake:workId").Value);
            #endregion

            #region Minio 配置
            // Add Minio using the default endpoint
            //builder.Services.AddMinio(accessKey, secretKey);

            // Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
            buil.Services.AddMinio(configureClient => configureClient
                .WithEndpoint(buil.Configuration["MinIO:Endpoint"]).WithSSL(false)
                .WithCredentials(buil.Configuration["MinIO:AccessKey"], buil.Configuration["MinIO:SecretKey"]));

            //var minioClient = new MinioClient().WithEndpoint(buil.Configuration["MinIO:Endpoint"]).WithCredentials(buil.Configuration["MinIO:AccessKey"], buil.Configuration["MinIO:SecretKey"]).WithSSL(true).Build();
            //buil.Services.AddSingleton(minioClient);
            #endregion

            #region 使用autofac
            buil.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            buil.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacModuleRegister());
            });
            #endregion

            #region 全局异常注册
            buil.Services.AddExceptionHandler<GlobalExceptionHandler>();
            buil.Services.AddProblemDetails();
            #endregion

            #region 运用缓存
            //初始化redis
            RedisHelper.redisClient.InitRedisConnect(buil.Configuration);
            buil.Services.AddCache(builder => builder.UseCache(buil.Configuration));
            #endregion



            #region
            //buil.Host.ConfigureContainer<ContainerBuilder>(builder =>
            //{
            //    builder.Register<ISqlSugarClient>(context =>
            //    {
            //        // ISqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            //        // {
            //        //     DbType = DbType.SqlServer,
            //        //     ConnectionString = "Data Source=.;Initial Catalog=ZhaoxiAdminDb1;Persist Security Info=True;User ID=sa;Password=sa",
            //        //     IsAutoCloseConnection = true
            //        // });

            //        ISqlSugarClient db = new SqlSugarClient(new List<ConnectionConfig>()
            //            {
            //                new ConnectionConfig(){
            //                    ConfigId = DBEnums.默认数据库,
            //                    DbType = DbType.SqlServer,
            //                    ConnectionString = "Data Source=.;Initial Catalog=ZhaoxiAdminDb1;Persist Security Info=True;User ID=sa;Password=sa",
            //                    IsAutoCloseConnection = true
            //                }

            //            }
            //        );

            //        //支持sql语句的输出，方便排查问题
            //        db.Aop.OnLogExecuted = (sql, par) =>
            //        {
            //            Console.WriteLine("\r\n");
            //            Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}，Sql语句：{sql}");
            //            Console.WriteLine("===========================================================================");
            //        };

            //        return db;
            //    });
            //    //注册接口和实现层
            //   builder.RegisterModule(new AutofacModuleRegister());
            //});
            #endregion

            #endregion

            #region 注册autuomapper
            // buil.Services.AddAutoMapper(typeof(AutoMapperConfigs),typeof(BatchMapperProfile));
            //批量自动映射
            buil.Services.AddAutoMapper(typeof(BatchMapperProfile));

            //添加 AutoMapper 的配置
            //使用AddAutoMapper()方法可以将AutoMapper所需的服务添加到该集合中，以便在应用程序的其他部分中使用。
            //该方法需要传入一个Assembly数组，以告诉AutoMapper要扫描哪些程序集来查找映射配置(在当前作用域的所
            //有程序集里面扫描AutoMapper的配置文件)。
            //buil.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region 开启cap
            buil.Services.AddCap(x =>
            {


                List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { "ConnectionConfigs" }) ?? throw new InvalidOperationException("数据库连接配置未找到");
                //var conn = connectionConfigs.Where(a => a.DbType == IocDbType.MySql).FirstOrDefault();

                var mysqlConfig = connectionConfigs.FirstOrDefault(c => c.DbType == IocDbType.MySql)
                   ?? throw new InvalidOperationException("未配置MySQL数据库连接");

                if (string.IsNullOrWhiteSpace(mysqlConfig.ConnectionString))
                {
                    throw new InvalidOperationException("MySQL连接字符串不能为空");
                }

                x.UseMySql(mysqlConfig.ConnectionString);

                // 绑定并验证RabbitMQ配置
                var rabbitMqSettings = new RabbitMqSettings();
                buil.Configuration.GetSection("RabbitMQ").Bind(rabbitMqSettings);
                // 验证RabbitMQ必要配置
                if (string.IsNullOrWhiteSpace(rabbitMqSettings.HostName))
                    throw new InvalidOperationException("RabbitMQ主机名未配置");

                if (string.IsNullOrWhiteSpace(rabbitMqSettings.UserName))
                    throw new InvalidOperationException("RabbitMQ用户名未配置");

                if (string.IsNullOrWhiteSpace(rabbitMqSettings.Password))
                    throw new InvalidOperationException("RabbitMQ密码未配置");

                // 配置RabbitMQ
                x.UseRabbitMQ(opt =>
                {
                    opt.HostName = rabbitMqSettings.HostName;
                    opt.UserName = rabbitMqSettings.UserName;
                    opt.Password = rabbitMqSettings.Password;
                    opt.Port = rabbitMqSettings.Port;
                    opt.VirtualHost = rabbitMqSettings.VirtualHost;
                });


                x.FailedRetryCount = 10;//重试次数
                x.FailedRetryInterval = 20;//多久重试一次，以秒为单位

                x.FailedThresholdCallback = failed =>
                {
                    // 建议添加日志记录

                    Log.Error("消息处理失败达到阈值: {MessageId}", failed.Message.GetId());
                    //  Log.LogError("消息处理失败达到阈值: {MessageId}", failed.MessageId);

                    // 可以在这里实现通知逻辑（邮件、短信等）
                };

            });
            #endregion

            #region 注册中心
            //buil.Services.AddConsul();
            #endregion

            #region JWT校验

            //第一步，注册JWT
            buil.Services.Configure<JWTTokenOptions>(buil.Configuration.GetSection("JWTTokenOptions"));
            //第二步，增加鉴权逻辑
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            buil.Configuration.Bind("JWTTokenOptions", tokenOptions);
            buil.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
             .AddJwtBearer(options =>  //这里是配置的鉴权的逻辑
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                     ValidateIssuer = true,//是否验证Issuer
                     ValidateAudience = true,//是否验证Audience
                     ValidateLifetime = true,//是否验证失效时间
                     ValidateIssuerSigningKey = true,//是否验证SecurityKey
                     ValidAudience = tokenOptions.Audience,//
                     ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey ?? "")),//拿到SecurityKey 
                     //RequireExpirationTime = true,//要求Token的Claims中必须包含Expires
                     ClockSkew = TimeSpan.Zero, //允许服务器时间偏移量300秒，即我们配置的过期时间加上这个允许偏移的时间值，才是真正过期的时间(过期时间 + 偏移值)你也可以设置为0，

                 };
             });//*/
            #endregion

            #region 跨域策略
            buil.Services.AddCors(options =>
            {
                //添加跨域策略
                options.AddPolicy("CorsPolicy",
                    opt => opt.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination"));

                #region 如果需要限定请求地址、媒体请求和标头等内容

                //options.AddPolicy("CorsPolicy", opt => opt.WithMethods("GET", "POST", "PUT", "DELETE")//允许GET、post,put,delete请求
                //.WithOrigins("http://localhost:8080")//允许http://localhost:8080的地址访问api,不通的端口也是跨域
                //.WithHeaders("Browser-Type")//允许header中包含Browser-Type参数
                //.WithExposedHeaders("X-Pagination")) ;
                #endregion
            });

            #endregion

            #region 配置定时任务

            buil.Services.AddQuartz(options =>
            {
                options.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();
                //options.UseMicrosoftDependencyInjectionJobFactory(); 已经弃用

                options.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 1;//单线程执行 多个数据库连接区域连接容易出现问题 
                });
                options.AddJobListener<CustomJobListener>();

            });
            buil.Services.AddQuartzHostedService(
                options =>
                {
                    // when shutting down we want jobs to complete gracefully
                    options.WaitForJobsToComplete = true;
                });
            buil.Services.UseQuartz();
            #endregion

            #region swagger文件显示注释信息

            //添加swagger
            buil.Services.AddSwaggerGen(option =>
             {
                 foreach (var controller in SwaggerUtil.GetControllers())
                 {
                     var groupname = SwaggerUtil.GetSwaggerGroupName(controller);

                     option.SwaggerDoc(groupname, new OpenApiInfo
                     {
                         Version = "v1",
                         Title = groupname,
                         Description = groupname + "接口定义详细信息"
                     });
                 }

                 foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.*",
                             SearchOption.AllDirectories).Where(f => Path.GetExtension(f).ToLower() == ".xml"))
                 {
                     option.IncludeXmlComments(name, includeControllerXmlComments: true);
                     // logger.LogInformation($"find api file{name}");
                 }

                 option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                 {
                     Description = "Value: Bearer {token}",
                     Name = "Authorization",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey,
                     Scheme = "Bearer"
                 });
                 option.AddSecurityRequirement(new OpenApiSecurityRequirement()
                 {
                   {
                     new OpenApiSecurityScheme()
                      {
                          Reference = new OpenApiReference()
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header

                     },
                     new List<string>()
                   }
                 });
             });

            #region 注释掉的代码

            //        buil.Services.AddSwaggerGen((options) =>
            //        {
            //            options.SwaggerDoc("v1", new OpenApiInfo { Title = "核心API", Version = "v1.0", Description = "接口定义详细信息", });
            //            //options.SwaggerDoc("v2", new OpenApiInfo { Title = "核心API2", Version = "v2.0", Description = "接口定义详细信息第二版本", });
            //            var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
            //            var xmlPath = Path.Combine(basePath ?? "", $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");//接口action显示注释
            //            options.IncludeXmlComments(Path.Combine(basePath ?? "", "Demo.WebAPI.xml"), true);//接口注释
            //            options.IncludeXmlComments(Path.Combine(basePath ?? "", "Demo.API.Application.xml"), true);//实体类注释



            //            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //            {
            //                Description = "Value: Bearer {token}",
            //                Name = "Authorization",
            //                In = ParameterLocation.Header,
            //                Type = SecuritySchemeType.ApiKey,
            //                Scheme = "Bearer"
            //            });
            //            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            //{
            //    {
            //        new OpenApiSecurityScheme()
            //        {
            //            Reference = new OpenApiReference()
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id="Bearer"
            //            },
            //            Scheme = "oauth2",
            //            Name = "Bearer",
            //            In = ParameterLocation.Header

            //        },
            //        new List<string>()
            //    }
            //});
            //        });

            #endregion


            #endregion

        }


        public static void UseAppRegister(this WebApplication app)
        {
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    // 按标签排序，即按控制器分组显示
                    //options.DefaultModelsExpandDepth(-1); // 不显示模型
                    //options.DisplayOperationId();
                    //options.EnableTryItOutByDefault();

                    //options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    //c.IndexStream = () =>
                    //           IntrospectionExtensions.GetTypeInfo(GetType()).Assembly
                    //               .GetManifestResourceStream("OpenAuth.WebApi.index.html");
                    //c.IndexStream = () =>
                    //   Assembly.GetExecutingAssembly()
                    //       .GetManifestResourceStream("laoyoutiao.webapi.index.html");
                    foreach (var controller in SwaggerUtil.GetControllers())
                    {
                        var groupname = SwaggerUtil.GetSwaggerGroupName(controller);

                        c.SwaggerEndpoint($"/swagger/{groupname}/swagger.json", groupname);
                    }

                    c.DocExpansion(DocExpansion.List); //默认展开列表
                    c.OAuthClientId("laoyoutiao.WebApi"); //oauth客户端名称
                    c.OAuthAppName("laoyoutiao权限认证"); // 描述
                });
            }
            app.UseRouting();

            #region 鉴权
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion

            #region 跨域设置
            app.UseCors("CorsPolicy");
            #endregion
            //启用定时任务
            UseTask.UseQuartz(app, app.Lifetime, app.Configuration);
            ServiceProviderInstance.Instance = app.Services;//.ApplicationServices;
            ServiceProviderInstance.wwwrootpath = app.Environment.WebRootPath;
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
        }
    }
}
