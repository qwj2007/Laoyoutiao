using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;

namespace Laoyoutiao.IService
{

    public interface IUserService : IBaseService<Users>
    {
        /// <summary>
        /// 登录用
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserRes GetUser(string userName, string password);       
       
        /// <summary>
        /// 根据id获取单个用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       // UserRes GetUsersById(long id);
        /// <summary>
        /// 设置角色
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="rids"></param>
        /// <returns></returns>
        bool SettingRole(string pid, string rids);
        /// <summary>
        /// 个人中心修改用户昵称或者密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool EditNickNameOrPassword(long? userId, string? nickName, string? password);
    }
}
