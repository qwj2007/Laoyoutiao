using Laoyoutiao.Models.Dto.Sys.DataPermission;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IDataPermissionService : IBaseService<DataPermission>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataId"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        DataPermissionRes  GetModel(long DataId, string DataType);
    }
}
