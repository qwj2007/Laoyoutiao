using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 图片验证码接口
    /// </summary>
    [ApiController]
    [Route("api/captcha")]
    [ApiExplorerSettings(GroupName = "校验验证码是否有效")]
    public class CaptchaController : ControllerBase
    {
        private readonly ICache _cache;
        public CaptchaController(ICache cache)
        {
            _cache = cache;
        }
        [HttpGet("image")]
        public async Task<IActionResult> GetCaptcha()
        {
            string code = CaptchaHelper.GenerateCode();
            //HttpContext.Session.SetString("CaptchaCode", code);
            // 将验证码存储在Redis中，设置过期时间为5分钟
            string captchaId = Guid.NewGuid().ToString().Replace("-", "");           
            _cache.WriteCache<string>(captchaId, code, TimeSpan.FromMinutes(5));
            Response.Headers["X-Captcha-Id"] = captchaId;
            byte[] bytes = CaptchaHelper.GenerateCaptchaImage(code);
            return File(bytes, "image/png");
        }
        /// <summary>
        /// 检查验证码是否有效
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="captchaId"></param>
        /// <returns></returns>
        [HttpPost("check")]
        public async Task<ApiResult> Check(string userCode, string captchaId)
        {
            var hasCache = false;
           
            var code = _cache.GetCache<string>(captchaId, out hasCache);
            if (string.IsNullOrEmpty(code) || !userCode.ToLower().Equals(code.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return ResultHelper.Error("验证码错误");
            }
            // 验证成功后删除验证码，防止重复使用
           
            _cache.RemoveCache(captchaId);
            return ResultHelper.Success(true);
        }
    }
}
