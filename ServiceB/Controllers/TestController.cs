using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ServiceB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("getServiceB")]
        public string GetServiceB()
        {
            return "Hello From ServiceB...........";
        }

        [HttpGet]
        [Route("getServiceB1")]
        public string GetServiceByName(string name)
        {
            return "Hello From ServiceB..........."+name;
        }
        [HttpPost]
        [Route("postServiceB")]
        public string PostServiceB() {
            return "Hello From PostSerivceB.........";
        }

        [HttpPost]
        [Route("postServiceB1")]
        public string PostServiceB(Users user)
        {
            return "Hello From PostSerivceB........." + user.name;
        }
    }
}

