using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        //private readonly string key = "My_test_key_is_here";
        private readonly IConfiguration _configuration;
        public JwtAuthenticationService(IConfiguration configuration)  
        {
            _configuration = configuration;
        }
        public string Authenticate(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: Get_Claims(user),
                expires: DateTime.Now.AddDays(2),
                signingCredentials: signingCredentials
                );

            return tokenHandler.WriteToken(tokenDescriptor);
            /*
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
      
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                }),
                Expires = DateTime.UtcNow.AddHours(48),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
            */

        }
        private static List<Claim> Get_Claims(IdentityUser user)
        {
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.Email, user.Email) ,
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };   //add id, add username
            return claims;
        }
    }
}
