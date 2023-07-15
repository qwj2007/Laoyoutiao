namespace Laoyoutiao.Models.Common
{
    /// <summary>
    /// JWT配置
    /// </summary>
    public class JWTTokenOptions
    {
        public string? Audience
        {
            get;
            set;
        }
        public string? SecurityKey
        {
            get;
            set;
        }
        public string? Issuer
        {
            get;
            set;
        }
    }
}
