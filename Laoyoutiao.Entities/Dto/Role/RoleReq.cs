using Laoyoutiao.Models.Common;

namespace Laoyoutiao.Models.Role
{
    public class RoleReq: Pagination
    {
        public string? Name { get; set; } 
        public bool IsEnable { get; set; }
        public string? Description { get; set; }       
    }
}
