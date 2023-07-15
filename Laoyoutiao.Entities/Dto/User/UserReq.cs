using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.User
{
    public class UserReq: Pagination
    {
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public int UserType { get; set; }
        public bool IsEnable { get; set; }
        public string? Description { get; set; }
       
    }
}
