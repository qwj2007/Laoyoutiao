using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
namespace Laoyoutiao.Service.Sys;


public class SysUserService : BaseService<SysUser>, ISysUserService
{
    private readonly IMapper _mapper;
    public SysUserService(IMapper mapper) : base(mapper)
    {
      _mapper=mapper;
    }
    /// <summary>
    ///获取到用户信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public SysUserRes GetUser(string Account, string password)
    {
        var user = _db.Queryable<SysUser>().Where(u => u.Account == Account && u.Password == password).First();
        if (user != null)
        {
            return _mapper.Map<SysUserRes>(user);
        }
        return new SysUserRes();
    }
}
