//using Laoyoutiao.Common;
//using Laoyoutiao.IService;
//using Laoyoutiao.IService.users;
//using Laoyoutiao.Models.Common;
//using Laoyoutiao.Models.Dto.User;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace Laoyoutiao.webapi.Controllers
//{
//    [ApiController]
//    [Route("[controller]/[action]")]
//    public class HomeController : ControllerBase
//    {        
//        private readonly IUserService _userService;      
//        public HomeController(IUserService usersService)
//        {
//            _userService = usersService;
//        }

//        [HttpGet]
//        public ApiResult GetUsersById(long id)
//        {
//            return ResultHelper.Success(_userService.GetEntityById(id));
//        }

//        [HttpPost]
//        public ApiResult Add(UserAdd req)
//        {
//            //获取当前登录人信息 
//            long userId = 0;
//#if DEBUG
//            Random random = new Random();
//            userId = random.Next(10, 999999);
//#else
//      userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
//#endif


//            return ResultHelper.Success(_userService.Add(req, userId));
//        }

//        [HttpPost]
//        public ApiResult GetUsers(UserReq req)
//        {
//            long userId = 3; //Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
//            return ResultHelper.Success(_userService.GetUsers("张三","123456"));
//        }
//        [HttpGet]
//        [Route("getNNN")]
//        public IActionResult GetNNN()
//        {
//            //_userService.GetAll();
//            var list =  _userService.GetAll();
//            return  Content(JsonConvert.SerializeObject(list));
//            //return Content("dfsdfsd");

//        }

//        //[HttpGet]
//        //[Route("getadd")]
//        //public async Task<IActionResult> GetAdd()
//        //{
//        //    var list = await _addressRepository.GetListAsync();
//        //    return Content(JsonConvert.SerializeObject(list));
//        //}

//        [HttpGet]
//        [Route("getuser")]
//        public async Task<IActionResult> getuser()
//        {

//            // var list = await _usersRepository.GetListAsync();
//            //_usersRepository.Test();
//            ////var list2 = await _addressRepository.GetListAsync();
//            //// var dd= _db.AsTenant();
//            //// var list = await dd.QueryableWithAttr<User>().ToListAsync();
//            //return Content(JsonConvert.SerializeObject(list) );
//            return Content("dfdsfd");
//        }

//        [HttpGet]
//        [Route("tt")]
//        public string tttd()
//        {
//            return "hello";
//        }
//    }
//}
