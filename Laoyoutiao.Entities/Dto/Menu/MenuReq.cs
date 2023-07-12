using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Menu
{
    public class MenuReq:Pagination
    {
        public string? Name { get; set; }
        public string Index { get; set; }
        public string FilePath { get; set; }
        public long ParentId { get; set; }
        public string Description { get; set; }
    }
}
