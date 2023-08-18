using Laoyoutiao.Models.CustomAttribute;

using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Menus))]
    public class MenusEdit: BaseDto
    {
        #region
        //public string? MenuName { get; set; }
        //public long SystemId { get; set; } = 0;
        //public long ParentId { get; set; }
        //public string? MenuUrl { get; set; }
        //public int Sort { get; set; } = 0;
        ////public string ButtonClass { get; set; } = "";        
        //public string? Icon { get; set; }
        //public int IsShow { get; set; }
        ////public int IsButton { get; set; }
        ////public string BtnType { get; set; } = "";
        //public string? ComponentUrl { get; set; }
        //public string? Code { get; set; }
        //public string? Memo { get; set; }

        //public string ButtonClass { get; set; } = "";

        //public int IsButton { get; set; } = 1;

        //public string BtnType { get; set; }
        #endregion


        public string? Name { get; set; }
    
       // public long SystemId { get; set; }
        // [SugarColumn(IsNullable = false, ColumnDescription = "上级菜单")]
        public long ParentId { get; set; }

        //public override long ParentId { get => base.ParentId; set => base.ParentId = value; }

  
        public string? MenuUrl { get; set; }
    
        public int Sort { get; set; }
  
        public string? ButtonClass { get; set; }
   
        public string? Icon { get; set; }

        public int IsShow { get; set; }

        public int IsButton { get; set; }


        public string? BtnType { get; set; }


        //public string Path { get; set; }

        public string? ComponentUrl { get; set; } = "0";
     
        public string? Code { get; set; } 
        public string? Memo { get; set; }
        public List<MenusEdit>? Children { get; set; }
    }
}
