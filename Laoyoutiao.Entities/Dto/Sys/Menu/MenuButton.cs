using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Menus))]
    public class MenuButton
    {
        public long Id { get; set; }
        
        [PropertyMapper(SourceName = "MenuName")]       
        public string BtnName { get; set; }       
        public long ParentId { get; set; }    
        public string ButtonClass { get; set; } = "";
        public string Icon { get; set; }
        public int IsButton { get; set; } = 1;
        public string BtnType { get; set; }   
        public string Code { get; set; }
    }
}
