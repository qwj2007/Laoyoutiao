using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys;

namespace Laoyoutiao.Models.Dto.User
{
    [TypeMapper(SourceType =typeof(Users))]
    public class UserAdd:BaseDto
    {
        [PropertyMapper(SourceName ="Name")]
        public string? Name { get; set; }
        [PropertyMapper(SourceName = "NickName")]
        public string? NickName { get; set; }
        public string? Password { get; set; }
        public bool IsEnable { get; set; }
        public string? Description { get; set; }
    }
}
