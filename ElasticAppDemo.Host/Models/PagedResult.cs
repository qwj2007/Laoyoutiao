namespace ElasticAppDemo.Host.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; set; }
        public bool HasMore { get; set; }
        public string ScrollId { get; set; } // 用于下一页查询的标识符
    }
}
