using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService
{
    public interface ICustomJWTService
    {
        //获取token
        string GetToken(UserRes user);
        //获取token
        string GetToken(SysUserRes user);
    }
}
