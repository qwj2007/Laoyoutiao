using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(DeptMent))]
    public class DeptRes
    {
        public long Id { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string Memo { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public long ParentId { get; set; }
        public string Path { get; set; }
        public List<DeptRes> Children { get; set; }
    }
}
