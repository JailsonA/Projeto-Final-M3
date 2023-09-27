using DataAccessLayer.Data;
using DataAccessLayer.Filters;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class LoginRepository : ILoginInterface
    {
        private readonly ClinicaDbContext _context;
        private readonly IGenTokenFilter _genTokenFilter;
        private readonly IConfiguration _configuration;

        public LoginRepository(ClinicaDbContext context, IGenTokenFilter genTokenFilter, IConfiguration configuration)
        {
            _context = context;
            _genTokenFilter = genTokenFilter;
            _configuration = configuration;
        }

        /*Login*/
        public string logIn(LoginModel login)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == login.Email.ToLower());

                if (user != null && user.PasswordIsValid(login.Password) && user.Status == 1)
                {
                    return _genTokenFilter.GenerateToken(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*Change PWD*/
        public T ChangePwd<T>(ChangePwdModel changePwd) where T : UserModel
        {
            var user = _context.Users.OfType<T>().FirstOrDefault(x => x.UserId == changePwd.UserId);
            if (user == null) return null;
            if (!user.PasswordIsValid(changePwd.OldPwd)) return null;
            if (changePwd.NewPwd != changePwd.ConfirmPwd) return null;
            if (user.PasswordIsValid(changePwd.NewPwd)) return null; // nova senha é igual à antiga

            user.Password = changePwd.NewPwd;
            user.SetPasswordHash();
            user.DateATT = DateTime.Now;

            _context.Update(user);
            _context.SaveChanges();
            return user;
        }

        /*Forgot PWD*/
        public async Task<bool> ForgotPwd<T>(ForgotPwdModel forgotPwd) where T : UserModel
        {
            var user = _context.Users.OfType<T>().FirstOrDefault(x => x.Email.ToLower() == forgotPwd.Email.ToLower());
            if (user == null) return false;

            user.Password = user.SetNewPassword(user.Password);
            string envEmail = user.Password;
            user.SetPasswordHash();
            user.DateATT = DateTime.Now;
            _context.Update(user);
            emailTemplate emailTemplate = new emailTemplate();
            string message = emailTemplate.resTpwd(envEmail);
            string subject = "Your New Password";
            Email email = new Email();
            var emailConfig = new Email.Econfig
            {
                _host = _configuration["SMTP:Host"],
                _username = _configuration["SMTP:Username"],
                _senha = _configuration["SMTP:Senha"],
                _porta = _configuration["SMTP:Porta"]
            };

            bool isSend = await email.SendEmail(forgotPwd.Email.ToString(), message, subject, emailConfig);
            if (isSend)
            {
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
