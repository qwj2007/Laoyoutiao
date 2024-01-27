using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System.Web;

namespace Laoyoutiao.Models.Dto.Sys.DicData
{
    [TypeMapper(SourceType = typeof(SysDicData))]
    public class DicDataRes
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DicCode { get; set; }
        public int Is_System { get; set; }
        public long ParentId { get; set; }
    }
}
