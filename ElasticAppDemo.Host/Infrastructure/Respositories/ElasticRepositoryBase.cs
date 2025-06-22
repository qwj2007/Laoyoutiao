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
        /// 批量插入文档 使用 BulkAsync（灵活但代码较多）
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        // Fix for the CS1061 error: The "BulkDescriptor" class does not have an "Id" method.
        // The issue arises because the "Id" method is being incorrectly called on the "BulkDescriptor" object.
        // Instead, the "Id" method should be called on the operation being added to the "BulkDescriptor".

        public virtual async Task<bool> BulkInsert(List<T> documents, string indexName = null)
        {
            var index = indexName ?? this.IndexName;
            var bulkDescriptor = new BulkDescriptor();
            foreach (var document in documents)
            {
                bulkDescriptor.Index<T>(i =>
                {
                    var operation = i.Index(index).Document(document);
                    // If the entity class has an "Id" property, use it to set the ID for the operation.
                    if (document.GetType().GetProperty("Id") != null)
                    {
                        var idValue = document.GetType().GetProperty("Id")?.GetValue(document)?.ToString();
                        if (!string.IsNullOrEmpty(idValue))
                        {
                            operation.Id(idValue); // Correctly set the ID on the operation.
                        }
                    }
                    return operation;
                });
            }

            var bulkResponse = await this.Client.BulkAsync(bulkDescriptor);
            if (bulkResponse.Errors)
            {
                // Handle errors
                foreach (var item in bulkResponse.ItemsWithErrors)
                    Console.WriteLine($"Failed to index {item.Id}: {item.Error.Reason}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 批量插入文档 使用 IndexManyAsync（简洁高效）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task<bool> IndexManyInsert(List<T> documents, string indexName = null)
        {
            var response = await this.Client.IndexManyAsync(documents,indexName ?? this.IndexName);

            // 部分失败处理
            if (response.Errors)
            {
                foreach (var item in response.ItemsWithErrors)
                    Console.WriteLine($"Error on document {item.Id}: {item.Error.Reason}");
                return false;
            }
            return true;
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
