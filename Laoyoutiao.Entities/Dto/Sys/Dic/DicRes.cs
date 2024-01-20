using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Models.Dto.Sys.Dic
{
    [TypeMapper(SourceType = typeof(SysDic))]
    public class DicRes
    {
        public long Id { get; set; }
        public string? Title { get; set; }           
        public long ParentId { get; set; }    
        public string? DicCode { get; set; }
        public string? DicType { get; set; }
        /// <summary>
        /// 下拉树是否展开
        /// </summary>
        public bool spread { get; set; } = true;
        public List<DicRes>? Children { get; set; }
    }
}
