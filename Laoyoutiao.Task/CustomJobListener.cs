using Laoyoutiao.Common;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service;
using Quartz;
using Quartz.Listener;
using Serilog;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Laoyoutiao.Tasks.Core
{
    /// <summary>
    /// 任务监听
    /// </summary>
    public class CustomJobListener : JobListenerSupport
    {
        private ISqlSugarClient _sqlSugarClient = null;
        public CustomJobListener()
        {
            string configId = "0";
            string ConnectionConfigs = "ConnectionConfigs";
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            var attr = typeof(SysTask).GetCustomAttribute<TenantAttribute>();
            if (attr != null) {
                var attrConfigId = attr.configId??"0";                
                configId = attrConfigId.ToString();                
            }
            _sqlSugarClient = DbScoped.SugarScope.GetConnection(configId ?? "0");            
        }
        public override string Name => "CustomJobListener";
        public override async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            //return base.JobExecutionVetoed(context, cancellationToken);
            await Console.Out.WriteLineAsync("JobExecutionVetoed");
        }
        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            var jobid = context.JobDetail.Key.Name;
            //Console.WriteLine(context.JobDetail.Key.Name);
            var runtime = GetTime(context.JobRunTime.TotalMilliseconds);
            var NextTime = context.NextFireTimeUtc?.LocalDateTime;
            var RunStartTime = context.FireTimeUtc.LocalDateTime;

            Regex _regex = new Regex("[0-9]");
            if (_regex.IsMatch(jobid))
            {
                if (long.TryParse(jobid, out long joid))
                {
                    // 更新数据库运行时间
                    try
                    {

                        ////using (var _sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
                        ////{
                        ////    ConnectionString = _configuration.GetConnectionString("Defaultcon"),
                        ////    DbType = (DbType)(System.Enum.Parse(typeof(DbType), _configuration.GetConnectionString("DBType")) ?? DbType.MySql),
                        ////    InitKeyType = InitKeyType.Attribute

                        ////}))
                        ////{
                        ////    await _sqlSugarClient.Ado.ExecuteCommandAsync($"update C_Base_Tasks set RunTime='{runtime}',NextTime='{NextTime}',Counts=Counts+1 where Id={joid}");
                        //    //await _sqlSugarClient.Updateable<C_Base_Tasks>().SetColumns(t => new C_Base_Tasks()
                        //    //{
                        //    //    Id = joid,
                        //    //    RunTime = runtime,
                        //    //    NextTime = NextTime,
                        //    //    //RunStartTime = RunStartTime,

                        //    //}).SetColumns(t => t.Counts == t.Counts + 1).Where(t => t.Id == joid).ExecuteCommandAsync();
                        //}
                        await _sqlSugarClient.Updateable<SysTask>().SetColumns(t => new SysTask()
                        {
                            Id = joid,
                            RunTime = runtime,
                            NextTime = NextTime,
                            //RunStartTime = RunStartTime,

                        }).SetColumns(t => t.Counts == t.Counts + 1).Where(t => t.Id == joid).ExecuteCommandAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"任务名称：{jobid}错误：{ex}");
                        await Task.FromResult(ex);
                    }

                }
            }
            //#if DEBUG
            Log.Information($"任务名称：{jobid}运行时间：{runtime},本次开始时间:{RunStartTime}下次运行时间：{NextTime}");
            //Console.WriteLine($"任务名称：{jobid}运行时间：{runtime},本次开始时间:{RunStartTime}下次运行时间：{NextTime}");
            //#endif



            await Task.FromResult("");
        }
        public override async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            await Task.FromResult("");
        }
        private string GetTime(double times)
        {
            if (times < 1000)
            {
                return $"{Math.Round(times, 2)}ms";
            }
            else
            {
                return $"{Math.Round(times / 1000, 2)}s";
            }
        }
    }
}
