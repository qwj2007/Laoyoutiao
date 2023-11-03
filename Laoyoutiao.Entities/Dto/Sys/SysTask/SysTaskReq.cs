using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.SysTask
{
    public class SysTaskReq:Pagination
    {
        public string? TaskName { get; set; }
    }
}
