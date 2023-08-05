using Laoyoutiao.Models.Common;

namespace Laoyoutiao.Models.Dto.Sys;

public class SysUserReq : Pagination
{
    public string? UserName { get; set; }
    public int IsDeleted { get; set; }
}
