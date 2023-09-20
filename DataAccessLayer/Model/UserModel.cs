using DataAccessLayer.Data.Enum;
using DataAccessLayer.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
        public int Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? DateATT { get; set; }

        /*encarregar o model com a responsabilidade de gerenciar as senhas*/
        public bool PasswordIsValid(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }

        public void SetPasswordHash()
        {
            Password = Password.GerarHash();
        }

        public string SetNewPassword(string password)
        {
            string newPassword = Guid.NewGuid().ToString().Substring(0, 6);
            Password = newPassword.GerarHash();
            return newPassword;
        }
    }
}
