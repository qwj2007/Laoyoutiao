using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.Dic
{
    public class DicReq : Pagination
    {       
        /// <summary>
        /// 查找的条件数据
        /// </summary>
        public string? Title { get; set; }

        
    }
}
