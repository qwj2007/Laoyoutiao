using ConsulServerCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceA.Hystrix.Impl;

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestHystrix _testHystrix;
        public TestController(TestHystrix testHystrix) {
            _testHystrix = testHystrix;
        }
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
        public async Task<string>  PostFromServiceB1(Users users)
        {
            var test = await _testHystrix.getDemo(users);
            return test;// HttpMicroService.HttpPost("ServiceB", "api/test/postServiceB1",JsonConvert.SerializeObject( users));
        }
    }
}
