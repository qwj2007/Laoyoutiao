using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.DicData
{
    public class DicDataReq:Pagination
    {
        public string Name { get; set; }
    }
}
