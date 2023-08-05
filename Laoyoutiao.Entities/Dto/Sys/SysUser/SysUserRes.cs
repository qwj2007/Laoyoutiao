using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Models.Dto.Sys;

[TypeMapper(SourceType = typeof(SysUser))]
public class SysUserRes
{

    public long Id { get; set; }

    public string? Account { get; set; }

    /// <summary>
    /// ÐÕÃû
    /// </summary>
    /// <value></value>
    public string? UserName { get; set; }

    /// <summary>
    /// ¹¤ºÅ
    /// </summary>
    /// <value></value>
    public string? JobNumber { get; set; }

    /// <summary>
    /// µÇÂ¼ÃÜÂë
    /// </summary>
    /// <value></value>
    public string? Password { get; set; }

    /// <summary>
    /// Í·Ïñ
    /// </summary>
    /// <value></value>
    public string? HeadImg { get; set; }

    public DateTime CreateDate { get; set; }
    public string Status { get; set; }
}
