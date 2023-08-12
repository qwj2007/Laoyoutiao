using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    public class SystemsReq: Pagination
    {
        public string SystemName { get; set; }
    }
}
