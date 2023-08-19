using Laoyoutiao.Models.CustomAttribute;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Laoyoutiao.Models.Entitys.Sys.UserDept))]
    public class UserDeptEdit:BaseDto
    {
        public long UserId { get; set; }
        public long DeptId { get; set; }
    }
}
