
using Laoyoutiao.Caches;
using Laoyoutiao.Enums;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service
{
    public class CustomJWTService : ICustomJWTService
    {
        private readonly JWTTokenOptions _JWTTokenOptions;
        protected readonly CurrentUserCache _customcache;
        public CustomJWTService(IOptionsMonitor<JWTTokenOptions> optionsMonitor, CurrentUserCache customcache)
        {
            _JWTTokenOptions = optionsMonitor.CurrentValue;
            _customcache = customcache;
        }
        //获取token的方法
        public string GetToken(UserRes user)
        {
            var claims = new[] {
           new Claim("Id",user.Id.ToString()),
           new Claim("NickName",user.NickName),
           new Claim("Name",user.Name),
           new Claim("UserType",user.UserType.ToString()),
           new Claim("Password",user.Password.ToString())
           };
            //需要加密key
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _JWTTokenOptions.Issuer,
                audience: _JWTTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }

        public string GetToken(SysUserRes user)
        {
            TokenResult tokenResult = new TokenResult();
            tokenResult.AccessToken = GetAccessToken(user);
            tokenResult.RefreshToken = GetRefreshToken(user);

            //
            //_customcache.GetUserInfo();
            return JsonConvert.SerializeObject(tokenResult);
        }

        /// <summary>
        /// 验证token是否有效
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>        
        public ClaimsPrincipal ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,//是否验证Issuer
                ValidateAudience = true,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                ValidAudience = _JWTTokenOptions.Audience,//
                ValidIssuer = _JWTTokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey ?? "")),//拿到SecurityKey
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取刷新token，7天过期
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetRefreshToken(SysUserRes user)
        {
            Claim[] claims;
            SigningCredentials credentials;
            TokenParam(user, out claims, out credentials);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _JWTTokenOptions.Issuer,
                audience: _JWTTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_JWTTokenOptions.RefreshTokenExpiration),
                signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            //
            //_customcache.GetUserInfo();
            return returnToken;
        }
        /// <summary>
        /// 获取Accesstoken,30分钟过期
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetAccessToken(SysUserRes user)
        {
            Claim[] claims;
            SigningCredentials credentials;
            TokenParam(user, out claims, out credentials);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _JWTTokenOptions.Issuer,
                audience: _JWTTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_JWTTokenOptions.AccessTokenExpiration),
                signingCredentials: credentials
                );
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            //
            //_customcache.GetUserInfo();
            return returnToken;
        }

        private void TokenParam(SysUserRes user, out Claim[] claims, out SigningCredentials credentials)
        {
            claims = new[] {
           new Claim("Id",user.Id.ToString()),
           new Claim("Account",user.Account),
           new Claim("UserName",user.UserName),
           new Claim("Password",user.Password.ToString())
           };
            //需要加密key
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));
            credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}

