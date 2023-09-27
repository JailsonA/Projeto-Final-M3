using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Model;
using Microsoft.Extensions.Logging; // Adicionado para ILogger
using DataAccessLayer.Interface;
using DataAccessLayer.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;

namespace eConsultas_API.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IDecToken _decToken;
        private readonly ILoginInterface _loginRepository;
        private readonly ClinicaDbContext _context;

        public LoginController(ILogger<UsersController> logger, IDecToken decToken, ILoginInterface loginRepository, ClinicaDbContext context)
        {
            _logger = logger;
            _decToken = decToken;
            _loginRepository = loginRepository;
            _context = context;
        }


        /* Login Section */

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var isLog = _loginRepository.logIn(login);
                    if (isLog != null)
                    {
                        return Ok(isLog);
                    }
                    else
                    {
                        return NotFound("Invalid user or password");
                    }
                }
                else
                    return NotFound("Invalid user or password");
            }
            catch (Exception)
            {

                throw;
            }
        }


        [UserAcess]
        [HttpPost]
        public IActionResult GetUserLogged([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                // Extrai o token do cabeçalho
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    return Ok(_decToken.GetLoggedUser(token));
                }
            }
            return BadRequest("Invalid token");
        }

        [UserAcess]
        [HttpPost]
        public IActionResult ChangePwd([FromBody] ChangePwdModel changePwd, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var loggedUser = _decToken.GetLoggedUser(token);

            if (loggedUser == null) return NotFound("Invalid user or password");

            var user = _context.Users.FirstOrDefault(x => x.UserId == loggedUser.UserId);

            if (user == null) return NotFound("User not found");

            changePwd.UserId = user.UserId;
            string msg = "Change Password: " + user.UserId;
            if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");

            return Ok(_loginRepository.ChangePwd<UserModel>(changePwd));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPwd([FromBody] ForgotPwdModel forgotPwd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _loginRepository.ForgotPwd<UserModel>(forgotPwd));
        }

        /*logger*/
        private bool loggers(string msg, string authorizationHeader)
        {
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                // Extrai o token do cabeçalho
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    var user = _decToken.GetLoggedUser(token);
                    if (user == null) return false;

                    string userId = user.UserId.ToString();
                    string obs = msg;
                    _logger.LogInformation("UserId: {UserId} acess {Obs} ", userId, obs);
                    return true;
                }
            }
            return false;
        }

    }


}
