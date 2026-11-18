using CoreModels.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace CoreModels
{
    public class GenearteToken
    {
        private readonly IConfiguration _configuration;

        public GenearteToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenearteJwtToken(Users user, string userRole)
        {
            var claims = new List<Claim>()
            {
                // idetify the email as subject
                new Claim(JwtRegisteredClaimNames.Sub, user.EmailId),

                //custom claim
                new Claim("UserId", user.UserId),

                //standard email claim
                new Claim(ClaimTypes.Email, user.EmailId),

                //Role claim for authorization 
                new Claim(ClaimTypes.Role, userRole),

                //unique token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //convert secret key from json to bites

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //how the token will be signed
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //create the token with issuer,auience,claim
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
