using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Entitys.Sys;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;

namespace Laoyoutiao.Tasks.Core
{
    public static class UseTask
    {
        /// <summary>
        /// 初始化定时任务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appLifetime"></param>
        /// <param name="Configuration"></param>
        /// <param name="jobs">额外附加的任务</param>
        public static void UseQuartz( IApplicationBuilder app, IHostApplicationLifetime appLifetime, IConfiguration Configuration, List<JobEntity> joblist = null)
        {
            #region 初始化任务
            appLifetime.ApplicationStarted.Register(() =>
            {
                var _schedulerFactory = app.ApplicationServices.GetRequiredService<ISchedulerFactory>();

                var _IScheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();

                while (_IScheduler.IsStarted)
                {
                    string configId = "0";
                    var ConnectionString = Configuration.GetConnectionString("Defaultcon");
                    string ConnectionConfigs = "ConnectionConfigs";
                    List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
                    var attr = typeof(SysTask).GetCustomAttribute<TenantAttribute>();
                    if (attr != null)
                    {
                        var attrConfigId = attr.configId ?? "0";
                        configId = attrConfigId.ToString();
                    }

                    List<SysTask> tasks = new List<SysTask>();
                    var _sqlSugarClient = DbScoped.SugarScope.GetConnection(configId ?? "0");
                    tasks = _sqlSugarClient.Queryable<SysTask>().Where(t => t.IsDeleted == 0).ToList();                   

                    //using (var db = new SqlSugarClient(new ConnectionConfig()
                    //{
                    //    ConnectionString = ConnectionString,
                    //    DbType = (DbType)(System.Enum.Parse(typeof(DbType), Configuration.GetConnectionString("DBType")) ?? DbType.MySql),
                    //    InitKeyType = InitKeyType.Attribute
                    //}))
                    //{
                    //    tasks = db.Queryable<SysTask>().Where(t => t.IsDeleted == 0).ToList();

                    //}


                    var _customtask = app.ApplicationServices.GetRequiredService<TaskHelper>();

                    #region 数据库任务
                    var Jobdll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Laoyoutiao.Jobs.dll");
                    var jobs = Jobdll.GetTypes()
                        .Where(t => t.IsClass && t.GetInterfaces()
                           .Contains(typeof(IJob))
                           )
                        .ToArray();
                    foreach (var data in tasks)
                    {
                        var job = jobs.FirstOrDefault(t => t.Name.Equals(data.TaskName, StringComparison.OrdinalIgnoreCase));
                        if (job != null)
                        {
                            _customtask.AddDataJob(data.Id.ToString(), job, data.Cron).GetAwaiter().GetResult();
                        }
                    }

                    #endregion

                    #region 基础任务
                    _customtask.AddJob(nameof(FuJianJob), typeof(FuJianJob), "0 0 3 ? * 1-5").GetAwaiter().GetResult();
                    //每周日0点备份数据库
                    _customtask.AddJob(nameof(DBJob), typeof(DBJob), "0 0 0 ? * 7").GetAwaiter().GetResult();
                    try
                    {
                        if (joblist != null)
                        {
                            foreach (var item in joblist)
                            {
                                _customtask.AddJob(item.JobId, item.JobType, item.Cron).GetAwaiter().GetResult();
                            }
                        }
                    }
                    catch
                    {


                    }

                    #endregion

                    break;
                }




            });
            //return app;
            #endregion
        }

        public static void UseQuartz(this IServiceCollection services)
        {
            services.AddSingleton<TaskHelper>();
            
        }
    }
}
