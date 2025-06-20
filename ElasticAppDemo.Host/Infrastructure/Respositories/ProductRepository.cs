using ElasticAppDemo.Host.Models;
using Nest;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public class ProductRepository : ElasticRepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IElasticProxy elasticProxy) : base(elasticProxy) { }       

        protected override string IndexName => "products";

        public override async Task<Tuple<int, IList<Product>>> QueryAsync(int page, int limit)
        {
            var query = await this.Client.SearchAsync<Product>(x=>x.Index(this.IndexName)
                .From((page-1)*limit)
                .Size(limit)
                .Sort(x=>x.Descending(v=>v.ReleaseDate)));
            return new Tuple<int, IList<Product>>(Convert.ToInt32(query.Total),query.Documents.ToList());               
            
        }

        /// <summary>
        /// 条件查询 基于Term
        /// </summary>
        /// <param name="ean"></param>
        /// <returns></returns>

        public async Task<IList<Product>> QueryByEanAsync(string ean) {
            var result = await this.Client.SearchAsync<Product>(x=>x.Index(this.IndexName)
            .Query(q=>q.Term(p=>p.Ean,ean)));
            return result.Documents.ToList();
        }
        /// <summary>
        /// 多条件查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IList<Product>> QueryByEanOrNameAsync(string key)
        {
            var result = await this.Client.SearchAsync<Product>(x => x.Index(this.IndexName)
              .Query(q => q.Term(p => p.Ean, key) || q.Term(p => p.Name, key)));
            return result.Documents.ToList();
        }
        /// <summary>
        /// 查询name只筛选status='active'的product
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IList<Product>> GetActiveProductsByNameAsync(string key)
        {
            var result = await this.Client.SearchAsync<Product>(x => x.Index(this.IndexName)
              .Query(q => q.Term(p => p.Name, key) && q.Term(p => p.Status, "Active"))
              
              );
            return result.Documents.ToList();
        }
        /// <summary>
        /// 聚合统计 ，计算平均值，最大值，最小值。
        /// </summary>
        /// <returns></returns>
        public async Task<AggregateDictionary> QueryPriceAggAsAsync() {
            var result = await this.Client.SearchAsync<Product>(x => x.Index(this.IndexName)
            .Size(0) //代表不返回源数据
            .Aggregations(agg =>agg.Average("price_avg",avg=>avg.Field("price"))
             .Max("price_max",max=>max.Field("price"))
             .Min("price_min",min=>min.Field("price"))
            )
            );
            return result.Aggregations;
        }
        /// <summary>
        /// 聚合分组
        /// </summary>
        /// <returns></returns>
        public async Task<AggregateDictionary> QueryBrandAggAsync() {
            var searchResult = await this.Client.SearchAsync<Product>(x=>x.Index(this.IndexName)
            .Size(0)
            .Aggregations(agg=>agg.Terms("brandgroup",group=>group.Field("brand")))
            
            );  

            return searchResult.Aggregations;
        }
    }
    
    public interface IProductRepository : IElasticRepositoryBase<Product>
    {
        Task<IList<Product>> QueryByEanAsync(string ean);
        Task<IList<Product>> QueryByEanOrNameAsync(string key);
        Task<IList<Product>> GetActiveProductsByNameAsync(string key);

        Task<Nest.AggregateDictionary> QueryPriceAggAsAsync();
        Task<AggregateDictionary> QueryBrandAggAsync();
    }
}

