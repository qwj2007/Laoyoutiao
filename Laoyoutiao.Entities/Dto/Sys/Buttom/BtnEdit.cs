using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(SysButton))]
    public class BtnEdit : BaseDto
    {
        public long Id { get; set; }
        public string? BtnName { get; set; }

        public string? Memo { get; set; }

        public string? Icon { get; set; }
        public string? BtnCode { get; set; }
    }
}
