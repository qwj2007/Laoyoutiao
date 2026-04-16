using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportExcelToDB.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ExcelImportService _importService;

        public ImportController(ExcelImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("请上传Excel文件");

            // 检查扩展名
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("只支持 .xlsx 或 .xls 文件");

            try
            {
                using var stream = file.OpenReadStream();
                int count = await _importService.ImportExcelAsync(stream);
                return Ok(new { success = true, message = $"成功导入 {count} 条用户数据" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
