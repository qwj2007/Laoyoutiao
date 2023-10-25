using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Dto.WF.Instance;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.WF.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("WF")]
    public class WorkFlowInstanceController : BaseController<WF_WorkFlow_Instance, WorkFlowInstanceRes, WorkFlowInstanceReq, WorkFlowInstanceEdit>
    {
        public readonly IWorkFlowInstanceService _instance;
        public WorkFlowInstanceController(IWorkFlowInstanceService instance) : base(instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// 获取用户的待办信息
        /// </summary>
        /// <param name="req">传递的参数</param>
        /// <returns></returns>
        [HttpGet,HttpPost]
        public async Task<ApiResult> GetUserTodoListAsync(WorkFlowInstanceReq req)
        {
            var result = await _instance.GetUserTodoListAsync(req);
            return ResultHelper.Success(result);
        }
    }
}
