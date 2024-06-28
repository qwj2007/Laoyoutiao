using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys.Dic;
using Laoyoutiao.Models.Dto.Sys.DicData;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class SysDicDataService : BaseService<SysDicData>, ISysDicDataService
    {
        public SysDicDataService(IMapper mapper, CurrentUserCache cache) : base(mapper, cache)
        {
        }
        public override Task<SysDicData> AddOrUpdateReturnEntity<TEdit>(TEdit input)
        {
            var dicDataEdit = input as DicDataEdit;
            if (string.IsNullOrEmpty(dicDataEdit.DicCode)) {
                //如果是空的系统自动生成一个编码
                dicDataEdit.DicCode = SnowFlakeSingle.Instance.NextId().ToString();
            }

            return base.AddOrUpdateReturnEntity(dicDataEdit);
        }
    }
}
