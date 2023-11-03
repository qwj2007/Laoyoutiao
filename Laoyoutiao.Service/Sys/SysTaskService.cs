using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class SysTaskService : BaseService<SysTask>, ISysTaskService
    {
        public SysTaskService(IMapper mapper) : base(mapper)
        {

        }

        public Task<bool> AddDataJob(string jobName, Type jobType, string cron)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddJobAsync(string JobName, string JobGroup, string JobNamespaceAndClassName, string JobAssemblyName, string CronExpress)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveJob(string jobName, string jobGroup)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveJob(string jobName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopJobAsync(string jobName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopJobAsync(string jobName, string jobGroups)
        {
            throw new NotImplementedException();
        }
    }
}
