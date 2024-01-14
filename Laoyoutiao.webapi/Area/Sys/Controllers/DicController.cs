using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
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
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseService"></param>
        public DicController(IBaseTreeService<SysDic> baseService) : base(baseService)
        {
        }
    }
}
