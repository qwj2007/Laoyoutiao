using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys.Dic;
using Laoyoutiao.Models.Dto.Sys.DicData;
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
    public class DicDataController : BaseController<SysDicData, DicDataRes, DicDataReq, DicDataEdit>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseService"></param>
        public DicDataController(IBaseService<SysDicData> baseService) : base(baseService)
        {
        }
       
    }
}
