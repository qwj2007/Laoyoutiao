using ElasticAppDemo.Host.Infrastructure.Respositories;
using ElasticAppDemo.Host.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ElasticAppDemo.Host.Controllers
{
    /// <summary>
    /// 笔记相关操作
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;
        public NoteController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// 新增索引并添加文档，如果文档有，就更新，没有就创建
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Note note)
        {
            await _noteRepository.AddAsync(note);
            return Ok("Success");
        }
        /// <summary>
        /// 查询笔记
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页数量</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QueryAsync(int page = 1, int limit = 10)
        {
            var result = await _noteRepository.QueryAsync(page, limit);
            return Ok(new
            {
                total = result.Item1,
                items = result.Item2
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] Note log)
        {
            await _noteRepository.UpdateAsync(log);
            return Ok("Success");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([Required] string id)
        {
            await _noteRepository.DeleteAsync(id);
            return Ok("Success");
        }
        /// <summary>
        /// 查询结果中包含指定昵称的笔记，并高亮显示
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> QueryByNameWithHighlightAsync([Required] string key)
        {
            var result = await _noteRepository.QueryByNameWithHighlightAsync(key);
            return Ok(result);
        }
        /// <summary>
        /// 查询多个字段都要匹配--and查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> QueryByNameAsync(string title, string name)
        {
            var result = await _noteRepository.QueryByNameAsync(title, name);
            return Ok(result);
        }
        /// <summary>
        /// 查询多个字段都要匹配--or查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IList<Note>> QueryByTitleOrNameAsync(string name)
        {
            var result = await _noteRepository.QueryByTitleOrNameAsync(name);
            return result;
        }

        /// <summary>
        /// 查询多个字段包括keyword,模糊查询 和or查询一样
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IList<Note>> QueryByMutiMatchAsync(string keyword)
        {
            var result = await _noteRepository.QueryByMutiMatchAsync(keyword);
            return result;
        }

        /// <summary>
        /// 自定义创建索引
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task CreateIndexWithCustomMappingAsync()
        {
            await _noteRepository.CreateIndexWithCustomMappingAsync();
        }
        [HttpPost]
        public async Task CreateIndexAsync()
        {
            await _noteRepository.CreateIndexAsync();
        }
        /// <summary>
        /// function_score自定义分数查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IList<Note>> QueryWithFunctionScoreAsync(string keyword)
        {
            var result = await _noteRepository.QueryWithFunctionScoreAsync(keyword);
            return result;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IList<NoteAgg>> GroupByNicknameAggAsync()
        {
            var result = await _noteRepository.GroupByNicknameAggAsync();
            return result;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        [HttpPost]

        public async Task<IList<NoteAgg>> GroupByNoteIdAggAsync()
        {
            var result = await _noteRepository.GroupByNoteIdAggAsync();
            return result;
        }

        [HttpPost]
        public async Task<IList<NoteAgg>> GroupByNoteAggAsync()
        {
            var result = await _noteRepository.GroupByNoteAggAsync();
            return result;
        }
        /// <summary>
        /// 批量插入文档，测试使用IndexManyInsert方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndexManyInsertAsync()
        {
            var result = await _noteRepository.InsertManyDocument();
            if (result)
            {
                return Ok("批量插入成功");
            }
            else
            {
                return BadRequest("批量插入失败");
            }

        }
        /// <summary>
        /// 批量插入文档，测试使用BulkInsert方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BulkInsertAsync()
        {
            var result = await _noteRepository.BulkInsert();
            if (result)
            {
                return Ok("批量插入成功");
            }
            else
            {
                return BadRequest("批量插入失败");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ScrollSearchAsync()
        {
            var result = await _noteRepository.ScrollSearchAsync();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("查询失败");
            }
        }

       

        
        [HttpPost]
        public async Task<IActionResult> PaginateWithSearchAfterAsync()
        {
            await _noteRepository.PaginateWithSearchAfterAsync(
                pageSize: 20,
                processPage: pageData =>
                {
                    // 处理当前页数据（示例：打印到控制台）
                    Console.WriteLine($"--- 当前页数据 ---");
                    foreach (var doc in pageData)
                    {
                        Console.WriteLine($"ID: {doc.Id}, 时间: {doc.updateTime}, 内容: {doc.title}");
                    }
                },
                initialSort: sd => sd.Descending(d => d.Id).Descending(d => d.noteId) // 确保返回值
            );

            return Ok("分页查询完成");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetFirstPageAsync(int pageSize = 10)
        {
            return Ok(await _noteRepository.GetFirstPageAsync(pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> GetPageBySearchAfterAsync(string scrollId=null, int pageSize = 10) { 
        
            return Ok(await _noteRepository.GetPageBySearchAfterAsync(scrollId, pageSize));
        }
    }
}