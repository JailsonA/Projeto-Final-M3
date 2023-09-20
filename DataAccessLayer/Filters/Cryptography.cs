using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace DataAccessLayer.Filters
{
    public static class Cryptography
    {
        public static string GerarHash(this string valor)
        {
            // Generate a salt
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hash the value with the salt
            string hashedValue = BCrypt.Net.BCrypt.HashPassword(valor, salt);

            return hashedValue;
        }
    }
}
