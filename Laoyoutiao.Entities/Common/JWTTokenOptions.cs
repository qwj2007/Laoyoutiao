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
        /// <summary>
        /// AccessToken过期时间
        /// </summary>
        public int AccessTokenExpiration { get; set; }

        /// <summary>
        /// RefreshToken过期时间
        /// </summary>
        public int RefreshTokenExpiration { get; set; }
    }

    /// <summary>
    /// Token相关结果
    /// </summary>
    public class TokenResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
