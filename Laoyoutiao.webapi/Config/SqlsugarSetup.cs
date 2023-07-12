using Laoyoutiao.Configuration;
using Laoyoutiao.Models.Common;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using System.Reflection;
using Laoyoutiao.Common;

namespace Laoyoutiao.webapi.Config
{
    public static class SqlsugarSetup
    {
        //public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration,
        //    string dbName = "ConnectString")
        //{
        //    SqlSugarScope sqlSugar = new SqlSugarScope(new List<ConnectionConfig>()
        //    {
        //        new ConnectionConfig() { ConfigId = 1, ConnectionString = configuration.GetConnectionString("FirstConnectString"), DbType = DbType.SqlServer, IsAutoCloseConnection = true },
        //        new ConnectionConfig() { ConfigId = 2, ConnectionString = configuration.GetConnectionString("SecondConnectString"), DbType = DbType.SqlServer, IsAutoCloseConnection = true },
        //    },
        //        db =>
        //        {
        //            //单例参数配置，所有上下文生效       
        //            db.Aop.OnLogExecuting = (sql, pars) =>
        //            {
        //                Console.WriteLine(sql);//输出sql
        //            };

        //            //技巧：拿到非ORM注入对象
        //            //services.GetService<注入对象>();
        //        });
        //    services.AddHttpContextAccessor();
        //    services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        //}
        public static void AddSqlsugarSetup(string ConnectionConfigs = "ConnectionConfigs")
        {
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            //sqlsugar注册
            SugarIocServices.AddSqlSugar(connectionConfigs);
            //多租户日志打印设置/全局过滤器
            SugarIocServices.ConfigurationSugar(db =>
            {
                connectionConfigs.ForEach(item =>
                {

                    SetQueryFilter(db.GetConnection(item.ConfigId));
                    SetLog(db, item.ConfigId);
                });
            });
        }
        //日志
        private static void SetLog(SqlSugarClient db, string configid)
        {
            db.GetConnection(configid).Aop.OnLogExecuting = (sql, para) => {
                //var param = para.Select(it => it.Value).ToArray();
                //sql语句
                string sqlQuery = UtilMethods.GetSqlString(DbType.SqlServer, sql, para);
                Console.WriteLine("SQL语句："+sqlQuery+",执行时间"+DateTime.Now);
                Console.WriteLine();
            };
        }
        /// <summary>
        /// 添加全局过滤器
        /// </summary>
        /// <param name="provider"></param>
        private static void SetQueryFilter(SqlSugarProvider provider)
        {
            //添加全局过滤器
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Laoyoutiao.Service.dll");
            if (files.Length > 0)
            {
                Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity)).ToArray();
                foreach (var entityType in types)
                {
                    var lambda = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(
                        new[] { Expression.Parameter(entityType, "it") },
                        typeof(bool), $"{nameof(BaseEntity.IsDeleted)} ==  @0",
                        false);
                    provider.QueryFilter.Add(new TableFilterItem<object>(entityType, lambda, true)); //将Lambda传入过滤器
                }
            }
        }
    }
}
