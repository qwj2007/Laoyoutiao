using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.DataPermission
{
    [TypeMapper(SourceType = typeof(Laoyoutiao.Models.Entitys.Sys.DataPermission))]
    public class DataPermissionRes
    {
        public long Id { get; set; }
        public string DataType { get; set; }
        public long DataRange { get; set; }
        public long DataId { get; set; }
        public string Depts { get; set; }
    }
}
