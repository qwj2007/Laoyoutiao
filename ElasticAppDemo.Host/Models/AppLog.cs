namespace ElasticAppDemo.Host.Models
{
    public class AppLog:ElasticModelBase
    {
        public string LogLevel { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public AppLog()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public AppLog(string id)
        {
            this.Id = id;
        }
    }
}
