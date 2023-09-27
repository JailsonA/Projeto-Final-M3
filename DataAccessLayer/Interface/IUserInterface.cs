using DataAccessLayer.Data.Enum;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserInterface
    {
        List<FileUser> GetImage();
        T AddUserGen<T>(T user) where T : UserModel;
        T UpdateUserGen<T>(T user) where T : UserModel;
        List<T> GetAllUsersGen<T>() where T : UserModel;
        T GetUserByIdGen<T>(int id) where T : UserModel;
        bool DeleteUserGen<T>(int id) where T : UserModel;
        T GetUserByEmailGen<T>(string email) where T : UserModel;
        bool IsFileCopy(FileUser image, int userId, int? appoint = null);
        List<DoctorModel> GetDoctorBy(string? region = null, string? city = null, string? Specialization = null);

    }
}
