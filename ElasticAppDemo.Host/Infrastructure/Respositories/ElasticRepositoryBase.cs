using ElasticAppDemo.Host.Models;
using Nest;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public abstract class ElasticRepositoryBase<T> : IElasticRepositoryBase<T> where T : ElasticModelBase
    {
        private readonly IElasticProxy _elasticProxy;
        public ElasticRepositoryBase(IElasticProxy elasticProxy)
        {
            _elasticProxy = elasticProxy;
        }
        protected abstract string IndexName { get; }
        protected IElasticClient Client => _elasticProxy.GetClient(IndexName);
        /// <summary>
        /// 创建索引，并添加文档。如果文档有，就更新，没有就创建
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(T item)
        {
            await this.Client.IndexAsync(item, x => x.Index(this.IndexName));
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateIndexAsync() {
            var existsResponse = await this.Client.Indices.ExistsAsync(this.IndexName);
            if (!existsResponse.Exists)
            {
                var createIndexResponse = await this.Client.Indices.CreateAsync(this.IndexName, c => c
                    .Map<T>(m => m
                        .AutoMap() // 自动映射Product属性
                    )
                );
                // 可根据需要检查 createIndexResponse.Acknowledged
                return createIndexResponse.Acknowledged;
            }
            return true; // 如果索引已存在，则返回true
        }
        /// <summary>
        /// 传入文档ID删除文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(string id)
        {
            await this.Client.DeleteAsync<T>(id, x => x.Index(this.IndexName));
        }
        /// <summary>
        /// 分页查找索引
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public virtual async Task<Tuple<int, IList<T>>> QueryAsync(int page, int limit)
        {
            var query = await this.Client.SearchAsync<T>(x => x.Index(this.IndexName)
            .From((page - 1) * limit)
            .Size(limit));
            return new Tuple<int, IList<T>>(Convert.ToInt32(query.Total), query.Documents.ToList());
        }
        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(T item)
        {
            //await this.Client.UpdateAsync<T>(item.Id, x => x.Index(IndexName));
            await this.Client.UpdateAsync<T>(item.Id, u => u
                .Index(this.IndexName)
                .Doc(item)
                .RetryOnConflict(3)); // 重试次数

            
        }
    }
}
