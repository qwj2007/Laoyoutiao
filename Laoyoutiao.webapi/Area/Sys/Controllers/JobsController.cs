using Autofac.Core;
using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys.SysTask;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Tasks.Core;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging.Signing;
using SqlSugar;
using System.Collections.Generic;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Area("Sys")]
    public class JobsController : BaseController<SysTask, SysTaskRes, SysTaskReq, SysTaskEdit>
    {
        private readonly SchedulerCenter _scheduleCenter;
        private readonly ISysTaskService _service;
        public JobsController(IBaseService<SysTask> baseService, SchedulerCenter scheduleCenter) : base(baseService)
        {
            _service = baseService as ISysTaskService;
            _scheduleCenter = scheduleCenter;
        }
        public override async Task<ApiResult> AddOrUpdateReturnEntity(SysTaskEdit req)
        {
            bool isok = false;
            var res=await base.AddOrUpdateReturnEntity(req);
            if (res.IsSuccess) {
                var model = res.Result as SysTask;
                var result = await _scheduleCenter.AddJobAsync(model.Id, req.TaskName, req.Groups, req.Cron);
                if (result.ResultCode == ResultCodeAddMsgKeys.CommonObjectSuccessCode)
                {
                    isok = true;
                    //model.Status = 1;
                    //isok = await _service.UpdateAsync(model);
                }
            }
            if (isok)
            {
                return ResultHelper.Success(isok);
            }
            else
            {
                return ResultHelper.Error("启动失败");
            }

        }
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task<ApiResult> StopJobAsync(long Id)
        {
            bool isok = false;
            var model = await _service.GetEntityByIdAsync(Id);
            if (model != null)
            {
                var result = await _scheduleCenter.StopJobAsync(model.Id.ToString(), model.Groups);
                if (result.ResultCode == ResultCodeAddMsgKeys.CommonObjectSuccessCode)
                {
                    model.Status = 0;//停止
                    isok = await _service.UpdateAsync(model);
                }
            }
            if (isok)
            {
                return ResultHelper.Success(isok);
            }
            else
            {
                return ResultHelper.Error("停止失败");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("/TaskInfo/Stop/")]
        //public async Task<ApiResult> StopJobAsync(long[] Id)
        //{
        //    var list = await _service.get<SysTaskRes>(Id);
        //    if (model != null)
        //    {
        //        list.ForEach(async x =>
        //        {
        //            await _scheduleCenter.StopJobAsync(x.Name, x.Group);
        //        });
        //        result = await _service.UpdateStatusByIdsAsync(Ids, (int)TaskInfoStatus.Stopped);
        //    }

        //    return JsonHelper.ObjectToJSON(result);
        //}

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> StartJobAsync(long Id)
        {
            bool isok = false;
            var model = await _service.GetEntityByIdAsync(Id);
            if (model != null)
            {

                var result = await _scheduleCenter.AddJobAsync(model.Id, model.TaskName,model.Groups, model.Cron);
                if (result.ResultCode == ResultCodeAddMsgKeys.CommonObjectSuccessCode)
                {
                    model.Status = 1;//启动
                    isok = await _service.UpdateAsync(model);
                }
            }
            if (isok)
            {
                return ResultHelper.Success(isok);
            }
            else
            {
                return ResultHelper.Error("启动失败");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> DeleteJobAsync(int Id)
        {
            bool isok = false;
            var model = await _service.GetEntityByIdAsync(Id);           
            if (model == null)
            {
               
            }
            else
            {
               var result= await _scheduleCenter.RemoveJob(Id.ToString(), model.Groups);
                if (result.ResultCode == ResultCodeAddMsgKeys.CommonObjectSuccessCode) {
                    isok = await _service.DelAsync(Id);
                }                  
            }
            if (isok)
            {
                return ResultHelper.Success(isok);
            }
            else
            {
                return ResultHelper.Error("启动失败");
            }
        }
    }
}
