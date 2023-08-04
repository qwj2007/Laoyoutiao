namespace Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Common;
using SqlSugar;


[SugarTable("sys_user")]
[Tenant("0")]
public class SysUser : BaseEntity
{

    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(IsNullable = false, Length = 50)]
    public string? Account { get; set; }
    [SugarColumn(IsNullable = false, Length = 50)]
    /// <summary>
    /// 姓名
    /// </summary>
    /// <value></value>
    public string? UserName { get; set; }
    [SugarColumn(IsNullable = false, Length = 50)]
    /// <summary>
    /// 工号
    /// </summary>
    /// <value></value>
    public string? JobNumber { get; set; }
    [SugarColumn(IsNullable = false, Length = 50)]
    /// <summary>
    /// 登录密码
    /// </summary>
    /// <value></value>
    public string? Password { get; set; }
    [SugarColumn(IsNullable = false, Length = 200)]
    /// <summary>
    /// 头像
    /// </summary>
    /// <value></value>
    public string? HeadImg { get; set; }
}
