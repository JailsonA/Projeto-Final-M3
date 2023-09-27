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

        /* Doctor Section */

        List<DoctorModel> GetDoctorBy(string? region = null, string? city = null, string? Specialization = null);
        // doctor por especialidade
        T AddUserGen<T>(T user) where T : UserModel;
        bool DeleteUserGen<T>(int id) where T : UserModel;
        List<T> GetAllUsersGen<T>() where T : UserModel;
        T GetUserByIdGen<T>(int id) where T : UserModel;
        T GetUserByEmailGen<T>(string email) where T : UserModel;
        T UpdateUserGen<T>(T user) where T : UserModel;
        ///List<DoctorModel> GetDoctorsBySpecialization(SpecializationEnum Specialization, RegionEnum region, CityEnum city);

        /* Patient Section */


        /* extra */
        bool IsFileCopy(FileUser image, int userId, int? appoint = null);
        //bool IsFile(FileUser image, int userId, int appointId);
        List<FileUser> GetImage();

    }
}
