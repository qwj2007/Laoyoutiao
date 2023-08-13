using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Menus))]
    public class MenusEdit: BaseDto
    {
        public string? MenuName { get; set; }
    }
}
