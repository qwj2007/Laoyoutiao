
using Laoyoutiao.Caches;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        protected readonly CustomCache _customcache;
        public CustomJWTService(IOptionsMonitor<JWTTokenOptions> optionsMonitor, CustomCache customcache)
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
            JwtSecurityToken token = new JwtSecurityToken(issuer: _JWTTokenOptions.Issuer, audience: _JWTTokenOptions.Audience, claims: claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }

        public string GetToken(SysUserRes user)
        {
            var claims = new[] {
           new Claim("Id",user.Id.ToString()),
           new Claim("Account",user.Account),
           new Claim("UserName",user.UserName),
           new Claim("Password",user.Password.ToString())
           };
            //需要加密key
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTTokenOptions.SecurityKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(issuer: _JWTTokenOptions.Issuer, audience: _JWTTokenOptions.Audience, claims: claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            //
            //_customcache.GetUserInfo();
            return returnToken;
        }
    }
}

