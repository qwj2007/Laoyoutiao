using ConsulServerCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("getService")]
        public string GetFromServiceB ()
        {
           return HttpMicroService.HttpGet("ServiceB", "api/test/getServiceB");
           
        }

        [HttpGet]
        [Route("getServiceb1")]
        public string GetFromServiceB1(string name)
        {
            return HttpMicroService.HttpGet("ServiceB", "api/test/getServiceB1?name=" + name);

        }

        [HttpPost]
        [Route("postServiceB")]
        public string PostFromServiceB1()
        {
            return HttpMicroService.HttpPost("ServiceB", "api/test/postServiceB");   
        }

        [HttpPost]
        [Route("postServiceB1")]
        public string PostFromServiceB1(Users users)
        {
            return HttpMicroService.HttpPost("ServiceB", "api/test/postServiceB1",JsonConvert.SerializeObject( users));
        }
    }
}
