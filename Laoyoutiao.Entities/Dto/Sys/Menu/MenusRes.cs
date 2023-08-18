using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Menus))]
    public class MenusRes
    {
        public long Id { get; set; }      
        public string Name { get; set; }     
        //public string SystemName { get; set; }        
        public long ParentId { get; set; }        
        public string MenuUrl { get; set; }
        public string ButtonClass { get; set; }      
        public string Icon { get; set; }
        public string IsShow { get; set; }       
        public string IsButton { get; set; } 
        public string ComponentUrl { get; set; }
        public string Code { get; set; }
        public string BtnType { get; set; }
        public int Sort { get; set; }
        public string Memo { get; set; }
       public List<MenusRes> Children { get; set; }
    }
}
