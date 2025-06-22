using ElasticAppDemo.Host.Models;
using Nest;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public interface INoteRepository : IElasticRepositoryBase<Note>
    {
        /// <summary>
        /// 查询笔记，内容高亮显示
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IList<Note>> QueryByNameWithHighlightAsync(string key);
        /// <summary>
        /// 多个字段查询and查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IList<Note>> QueryByNameAsync(string title, string name);

        /// <summary>
        /// 多个字段查询or查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IList<Note>> QueryByTitleOrNameAsync(string strSearchName);

        /// <summary>
        /// multi-match查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IList<Note>> QueryByMutiMatchAsync(string keyword);

        /// <summary>
        /// 自定义mapping创建索引
        /// </summary>
        /// <returns></returns>
        Task CreateIndexWithCustomMappingAsync();

        /// <summary>
        /// 自动创建索引
        /// </summary>
        /// <returns></returns>
        Task CreateIndexAsync();

        /// <summary>
        /// fucntion_score自定义分数查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IList<Note>> QueryWithFunctionScoreAsync(string keyword);
        /// <summary>
        /// 按发布者昵称分组统计笔记数量，并统计点赞、评论、收藏的总和
        /// </summary>
        /// <returns></returns>
        Task<IList<NoteAgg>> GroupByNicknameAggAsync();

        Task<IList<NoteAgg>> GroupByNoteIdAggAsync();

        Task<IList<NoteAgg>> GroupByNoteAggAsync();

        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <returns></returns>
        Task<bool> InsertManyDocument();

        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <returns></returns>
        Task<bool> BulkInsert();
    }
}
