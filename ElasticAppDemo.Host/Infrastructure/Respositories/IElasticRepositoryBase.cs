using ElasticAppDemo.Host.Models;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public interface IElasticRepositoryBase<T> where T : ElasticModelBase
    {
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(string id);
        Task<Tuple<int, IList<T>>> QueryAsync(int page, int limit);

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIndexAsync();


        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task<bool> BulkInsert(List<T> documents, string indexName = null);
        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task<bool> IndexManyInsert(List<T> documents, string indexName = null);
    }
}
