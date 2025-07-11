using Laoyoutiao.IService.Sys;
using Laoyoutiao.Service.Sys;
using Newtonsoft.Json;
using Quartz;
using SqlSugar;
using System.Text.Json.Serialization;

namespace Laoyoutiao.Jobs
{
    [DisallowConcurrentExecution] //此属性防止Quartz.NET试图同时运行相同的作业]
    public class DemoJob : IJob
    {
        //private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ISysRoleService _sysRoleService;

        public DemoJob(ISysRoleService sysRoleService)
        {
            _sysRoleService = sysRoleService;
        }
       
        public Task Execute(IJobExecutionContext context)
        {
            
            // 这里可以添加一些业务逻辑，比如调用服务层方法
            var roles = _sysRoleService.GetAllAsync().Result;
            // 如果需要将角色列表转换为JSON格式，可以使用JsonSerializer
           
           
            Console.WriteLine("DemoJob示例....：" + JsonConvert.SerializeObject(roles));
            return Task.CompletedTask;
        }
    }
}