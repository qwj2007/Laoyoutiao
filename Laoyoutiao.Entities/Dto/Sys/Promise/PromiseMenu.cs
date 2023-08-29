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
    public class PromiseMenu
    {
       
        public long Id { get; set; }       
        public string Path { get; set; }       
        public string Title { get; set; }      
        public string Component { get; set; }
        public string Icon { get; set; }
        public long ParentId { get; set; }
        public int IsButton { get; set; }
        public string Code { get; set; }
        public string ButtonClass { get; set; }
        public List<PromiseMenu> Children { get; set; } = null;
    }
}
