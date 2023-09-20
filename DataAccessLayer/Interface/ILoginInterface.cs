using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ILoginInterface
    {
        string logIn(LoginModel login);
        Task<bool> ForgotPwd<T>(ForgotPwdModel forgotPwd) where T : UserModel;
        T ChangePwd<T>(ChangePwdModel changePwd) where T : UserModel;

    }
}
