using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType =typeof(RoleMenu))]
    public class RoleMenuRes
    {
        public long RoleId { get; set; }
        public long MenuId { get; set; }
    }
}
