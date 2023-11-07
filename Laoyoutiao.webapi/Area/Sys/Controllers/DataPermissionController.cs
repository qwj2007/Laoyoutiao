using Laoyoutiao.Models.Dto.Sys.Buttom;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Laoyoutiao.Models.Dto.Sys.DataPermission;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.IService.Sys;
using Microsoft.AspNetCore.Mvc.Razor;
using Laoyoutiao.Common;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    ///  DataPermission 数据权限
    /// </summary>
    [Area("Sys")]
    public class DataPermissionController : BaseController<DataPermission, DataPermissionRes, DataPermissionReq, DataPermissionEdit>
    {/// <summary>
     /// 
     /// </summary>
     /// <param name="baseService"></param>
        private readonly IDataPermissionService _service;
        public DataPermissionController(IBaseService<DataPermission> baseService) : base(baseService)
        {
            _service = baseService as IDataPermissionService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataId"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        [HttpGet]
       public async Task<ApiResult> GetModel(long DataId, string DataType) {
            return ResultHelper.Success(_service.GetModel(DataId, DataType));
        }
    }
}
