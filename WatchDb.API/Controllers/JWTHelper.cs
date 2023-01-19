using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WatchDb.API.Models;

namespace WatchDb.API.Controllers
{
    public static class JWTHelper
    {
        public static JWTToken GetToken()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.Configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var jwtOptions = new JwtSecurityToken(
                issuer: ConfigurationManager.Configuration["JWT:ValidIssuer"],
                audience: ConfigurationManager.Configuration["JWT:ValidAudience"],
                claims: new List<Claim>(),
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(jwtOptions);

            return new JWTToken() { Token = tokenStr };
        }
    }
}
