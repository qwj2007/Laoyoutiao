using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Laoyoutiao.WorkFlow.Core;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EnumController : ControllerBase
    {
        /// <summary>
        /// 获取RejectType枚举
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetNodeRejectType()
        {
            PageInfo p = new PageInfo();
            var list = EnumHelper.EnumToDictionary<NodeRejectType>();
            p.data = list;
            p.total = list.Count;
            return  ResultHelper.Success(list);

        }
    }
}
