using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    //[TypeMapper(SourceType = typeof(Menus))]
    public class MenuButton
    {
        public long Id { get; set; }
        
        [PropertyMapper(SourceName = "MenuName")]       
        public string BtnName { get; set; }
        [PropertyMapper(SourceName = "ParentId")]
        public long ParentId { get; set; }
        [PropertyMapper(SourceName = "ButtonClass")]
        public string ButtonClass { get; set; } = "";
        [PropertyMapper(SourceName = "Icon")]
        public string Icon { get; set; }
        [PropertyMapper(SourceName = "IsButton")]
        public int IsButton { get; set; } = 1;
        [PropertyMapper(SourceName = "BtnType")]
        public string BtnType { get; set; }
        [PropertyMapper(SourceName = "Code")]
        public string Code { get; set; }
    }
}
