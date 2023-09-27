using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer.Filters
{
    public class UserAcess : ActionFilterAttribute
    {
        public string _secretKey { get; set; }
        private string _validIssuer { get; set; }
        private string _validAudience { get; set; }

        /* execute before the action requested */
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var secretKey = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Key"];
            var validIssuer = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"];
            var validAudience = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"];

            _secretKey = secretKey;
            _validIssuer = validIssuer;
            _validAudience = validAudience;

            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (ValidateToken(token))
                {
                    // Token válido, continue com a execução da ação
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }

        /* validate token from the request */
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(_secretKey);

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

        /* get validation parameters */
        private TokenValidationParameters GetValidationParameters(string secretKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _validAudience,
                ValidIssuer = _validIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        }

    }
}
