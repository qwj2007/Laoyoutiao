using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys.Dic;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("Sys")]
    public class DicController : BaseTreeController<SysDic, DicRes, DicReq, DicEdit>
    {
        private readonly ISysDicService _sysDicService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysDicService"></param>
        public DicController(ISysDicService sysDicService) : base(sysDicService)
        {
            _sysDicService = sysDicService;
        }

        [HttpPost]
        public async Task<ApiResult> DelDic(long Id)
        {
            var isok = await _sysDicService.DelAsync(Id);
            return ResultHelper.Success(isok);
        }
    }
}
