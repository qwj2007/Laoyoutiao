using ElasticAppDemo.Host.Infrastructure.Respositories;
using ElasticAppDemo.Host.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElasticAppDemo.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLogController : ControllerBase
    {
        private readonly IAppLogRepository _appLogRepository;
        public AppLogController(IAppLogRepository appLogRepository) {
            _appLogRepository = appLogRepository;
        }
        [HttpGet]
        public async Task<IActionResult> QueryAsync(int page = 1, int limit = 10)
        {
            var result = await _appLogRepository.QueryAsync(page, limit);
            return Ok(new
            {
                total = result.Item1,
                items = result.Item2
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] AppLog log)
        {
            await _appLogRepository.AddAsync(log);
            return Ok("Success");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] AppLog log)
        {
            await _appLogRepository.UpdateAsync(log);
            return Ok("Success");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([Required] string id)
        {
            await _appLogRepository.DeleteAsync(id);
            return Ok("Success");
        }
    }
}
