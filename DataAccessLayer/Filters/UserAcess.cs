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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var secretKey = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Key"];
            var validIssuer = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"];
            var validAudience = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"];
            //setar as props
            _secretKey = secretKey;
            _validIssuer = validIssuer;
            _validAudience = validAudience;

            // Verifique se o cabeçalho de autorização não está vazio e começa com "Bearer "
            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                // Extrai o token do cabeçalho
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (ValidateToken(token))
                {
                    // Token válido, continue com a execução da ação
                }
                else
                {
                    // Token inválido, retorne uma resposta não autorizada
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                // Cabeçalho de autorização inválido, retorne não autorizada
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }

        /* validar token OBS - pensar numa forma de nao repitir o codigo de validação!!!!*/
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
