using AutoMapper;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Dto.Role;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;

namespace Laoyoutiao.webapi.Config
{
    public class AutoMapperConfigs : Profile
    {
        public AutoMapperConfigs()
        {
            //角色
            CreateMap<Role, RoleRes>();
            //CreateMap<RoleAdd, Role>();
            CreateMap<RoleEdit, Role>();
            //用户
            CreateMap<Users, UserRes>();
           // CreateMap<UserAdd, Users>();
            CreateMap<UserEdit, Users>();
            //菜单
            CreateMap<Menu, MenuRes>();
            //CreateMap<MenuAdd, Menu>();
            CreateMap<MenuEdit, Menu>();

        }
    }
}
