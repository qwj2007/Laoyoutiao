using Laoyoutiao.Models.CustomAttribute;

namespace Laoyoutiao.Models.Dto.Sys.DataPermission
{
    [TypeMapper(SourceType = typeof(Laoyoutiao.Models.Entitys.Sys.DataPermission))]
    public class DataPermissionEdit: BaseDto
    {
        public string? DataType { get; set; }
        public long DataRange { get; set; }
        public long DataId { get; set; }
        public string? Depts { get; set; }
    }
}
