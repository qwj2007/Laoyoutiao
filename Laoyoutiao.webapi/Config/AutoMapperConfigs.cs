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
            CreateMap<Role, RoleRes>().ReverseMap();
            //CreateMap<RoleAdd, Role>();
            CreateMap<RoleEdit, Role>().ReverseMap();
            //用户
            CreateMap<Users, UserRes>().ReverseMap();
           // CreateMap<UserAdd, Users>();
            CreateMap<UserEdit, Users>().ReverseMap();
            //菜单
            CreateMap<Menu, MenuRes>().ReverseMap();
            //CreateMap<MenuAdd, Menu>();
            CreateMap<MenuEdit, Menu>().ReverseMap();

        }
    }
}
