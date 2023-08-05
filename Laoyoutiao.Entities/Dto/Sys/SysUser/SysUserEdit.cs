using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;
using System.Security.AccessControl;

namespace Laoyoutiao.Models.Dto.Sys;

[TypeMapper(SourceType = typeof(SysUser))]
public class SysUserEdit : BaseDto
{
    public string? UserName { get; set; }
    public string? Account { get; set; }
    public string? Password { get; set; }
    public string? JobNumber { get; set; }

}
