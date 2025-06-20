using ElasticAppDemo.Host.Models;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public interface IElasticRepositoryBase<T> where T:ElasticModelBase
    {
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(string id);
        Task<Tuple<int, IList<T>>> QueryAsync(int page,int limit);

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIndexAsync();
    }
}
