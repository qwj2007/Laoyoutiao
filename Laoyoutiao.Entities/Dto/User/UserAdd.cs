using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.User
{
    public class UserAdd:BaseDto
    {
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public string? Password { get; set; }
        public bool IsEnable { get; set; }
        public string? Description { get; set; }
    }
}
