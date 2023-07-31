using Autofac;
using Autofac.Extensions.DependencyInjection;
using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Laoyoutiao.webapi.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using MediatR;


namespace Laoyoutiao.Configuration
{
    public static class HostBuilderExtend
    {
        public static void Register(this WebApplicationBuilder buil)
        {
            //buil.Services.AddSingleton<IUsersRepository, UsersRepository>();
            buil.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            buil.WebHost.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                //添加Apollo配置中心
                //configurationBuilder.AddApollo(hostBuilderContext.Configuration.GetSection("apollo"))
                //    .AddNamespace("ApolloServiceConfig", ConfigFileFormat.Json).AddDefault();
            });

            // buil.Host.ConfigureAppConfiguration((context, b) =>
            // {
            //     //添加Apollo配置中心
            //     b.AddApollo(b.Build().GetSection("apollo"))
            //         //b.AddApollo(configuration.GetSection("apollo"))
            //         .AddDefault();
            // });

            #region  添加MediatR事件总线
            buil.Services.AddMediatR(Assembly.GetExecutingAssembly());
            #endregion
            #region 配置数据库
            #region 注入数据库

            buil.Services.AddSingleton(new AppSettings(buil.Configuration));
            SqlsugarSetup.AddSqlsugarSetup();

            #endregion

            #region 使用autofac
            buil.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            buil.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacModuleRegister());
            });
            #endregion

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
            //注册autuomapper
          //  buil.Services.AddAutoMapper(typeof(AutoMapperConfigs),typeof(BatchMapperProfile));

            buil.Services.AddAutoMapper( typeof(BatchMapperProfile));



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
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey ?? ""))//拿到SecurityKey 
                 };
             });//*/
            #endregion

            #region 跨域策略
            buil.Services.AddCors(options =>
            {
                //添加跨域策略
                options.AddPolicy("CorsPolicy", opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("X-Pagination"));

                #region 如果需要限定请求地址、媒体请求和标头等内容

                //options.AddPolicy("CorsPolicy", opt => opt.WithMethods("GET", "POST", "PUT", "DELETE")//允许GET、post,put,delete请求
                //.WithOrigins("http://localhost:8080")//允许http://localhost:8080的地址访问api,不通的端口也是跨域
                //.WithHeaders("Browser-Type")//允许header中包含Browser-Type参数
                //.WithExposedHeaders("X-Pagination")) ;
                #endregion
            });

            #endregion

            #region swagger文件显示注释信息
            buil.Services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "核心API", Version = "v1.0", Description = "", });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
                var xmlPath = Path.Combine(basePath ?? "", $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");//接口action显示注释
                options.IncludeXmlComments(Path.Combine(basePath ?? "", "Demo.WebAPI.xml"), true);//接口注释
                options.IncludeXmlComments(Path.Combine(basePath ?? "", "Demo.API.Application.xml"), true);//实体类注释


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Value: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
            #endregion

        }

    }
}
