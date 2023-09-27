using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DataAccessLayer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace DataAccessLayer.Filters
{
    public interface IDecToken
    {
        bool ValidateToken(string token);
  
        LoggedModel GetLoggedUser(string token);
    }

    public class DecToken : IDecToken
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger logger;
        public DecToken(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        /*Validar Token*/
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = ValParameters(_configuration["Jwt:Key"]);

            try
            {
                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine(jsonEx);
                return false;
            }
        }

        private TokenValidationParameters ValParameters(string secretKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        }

        /*Get User by token*/
        public LoggedModel GetLoggedUser(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || !ValidateToken(token))
                {
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ReadToken(token) as JwtSecurityToken;

                var userIdClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "UserId");
                var fullNameClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "FullName");
                var emailClaim = principal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.UniqueName);
                var userTypeClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "UserType");

                if (userIdClaim != null && fullNameClaim != null && emailClaim != null && userTypeClaim != null)
                {
                    return new LoggedModel
                    {
                        UserId = int.Parse(userIdClaim.Value),
                        FullName = fullNameClaim.Value,
                        Email = emailClaim.Value,
                        UserType = userTypeClaim.Value,
                        TokenKey = token
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
