using Laoyoutiao.Models.CustomAttribute;

using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Menus))]
    public class MenusEdit: BaseDto
    { 
        public string MenuName { get; set; }      
        public long SystemId { get; set; } = 0;  
        public long ParentId { get; set; }
        public string MenuUrl { get; set; }    
        public int Sort { get; set; } = 0;
        public string ButtonClass { get; set; } = "";        
        public string Icon { get; set; }
        public int IsShow { get; set; }    
        public int IsButton { get; set; }
     
        public string BtnType { get; set; }
        public string ComponentUrl { get; set; }
        public string Code { get; set; }
        public List<MenuButton> MenuButtons { get; set; } 
    }
}
