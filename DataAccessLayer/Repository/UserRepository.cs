using DataAccessLayer.Data;
using DataAccessLayer.Data.Enum;
using DataAccessLayer.Filters;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserInterface
    {
        private readonly ClinicaDbContext _context;
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly IGenTokenFilter _genTokenFilter;
        private readonly IDecToken _decToken;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> _logger, ClinicaDbContext context, IGenTokenFilter genTokenFilter, IDecToken decToken)
        {
            _context = context;
            _genTokenFilter = genTokenFilter;
            _secretKey = _genTokenFilter.GenerateRandomSecretKey(32);
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
            _decToken = decToken;
            _logger = _logger;
        }

        /* Doctor Sections */

        public List<DoctorModel> GetDoctorBy(string? region = null, string? city = null, string? Specialization = null)
        {
            var query = _context.Users.OfType<DoctorModel>();

            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(u => u.Region == region);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(u => u.City == city);
            }

            if (!string.IsNullOrEmpty(Specialization))
            {
                query = query.Where(u => u.Especialization == Specialization);
            }

            var doctors = query.ToList();
            return doctors;
        }



        /*Metodos Genericos Users*/
        // add user
        public T AddUserGen<T>(T user) where T : UserModel
        {
            // Verifique se já existe um usuário com o mesmo email
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                // Email duplicado, retorne uma resposta de erro
                throw new ArgumentException("Email already exists.");
            }

            user.CreationDate = DateTime.Now;
            user.Status = 1;
            user.SetPasswordHash();
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        // delete user
        public bool DeleteUserGen<T>(int id) where T : UserModel
        {
            var user = _context.Users.OfType<T>().FirstOrDefault(x => x.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // get all users
        public List<T> GetAllUsersGen<T>() where T : UserModel
        {
            var users = _context.Users.OfType<T>().ToList();
            return users;
        }

        // get user by id
        public T GetUserByIdGen<T>(int id) where T : UserModel
        {
            var user = _context.Users.OfType<T>().FirstOrDefault(x => x.UserId == id);
            return user;
        }

        // get doctor by email
        public T GetUserByEmailGen<T>(string email) where T : UserModel
        {
            var user = _context.Users.OfType<T>().FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            return user;
        }

        // Update user
        public T UpdateUserGen<T>(T user) where T : UserModel
        {
            var existingUser = GetUserByIdGen<T>(user.UserId);

            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.UserId} not found.");
            }

            // Usando reflexão para copiar os valores dos campos correspondentes
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name != "UserId" && property.CanWrite)
                {
                    if (property.Name != "Password")
                    {
                        // Copia o valor da propriedade, exceto se for "Password"
                        property.SetValue(existingUser, property.GetValue(user));
                    }
                }
            }

            existingUser.DateATT = DateTime.Now;

            _context.SaveChanges();

            return existingUser;
        }

        //Get all Image url by userId and return image path
        public List<FileUser> GetImage()
        {
            var userImg = _context.ImgUser.ToList();
            return userImg;
        }

        /* Save url File to Data base */
        public bool IsFileCopy(FileUser image, int userId, int? appoint = null)
        {
            if (image != null)
            {
                var imageUser = new FileUser
                {
                    ImageUrl = image.ImageUrl,
                    UserId = userId
                };
                _context.ImgUser.Add(imageUser);
                _context.SaveChanges();

                if (appoint != null)
                {
                    var pdfFile = _context.Appointments.FirstOrDefault(x => x.AppointId == appoint);
                    pdfFile.PDFFile = imageUser.ImgId.ToString();
                    _context.Appointments.Update(pdfFile);
                    _context.SaveChanges();
                }

                return true;
            }

            return false;
        }

    }
}
