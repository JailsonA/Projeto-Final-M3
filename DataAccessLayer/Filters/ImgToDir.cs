using DataAccessLayer.Data;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Filters
{
    public class ImgToDir
    {
        public string CopyFile(IFormFile imageFile, List<string> permExtensions, string uploadDirectory, ClinicaDbContext _context, int userId)
        {
            if (imageFile != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!permExtensions.Contains(extension))
                {
                    return null; // Extensão não permitida
                }

                fileName = fileName + "_" + DateTime.Now.ToString("yymmfff") + extension;
                var filePath = Path.Combine("wwwroot", uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Agora, você pode adicionar o registro ao banco de dados usando o EF Core e o dbContext fornecido
                var imageUser = new FileUser
                {
                    ImageUrl = Path.Combine(uploadDirectory, fileName),
                    UserId = userId
                };


                _context.ImgUser.Add(imageUser);
                _context.SaveChanges();

                return imageUser.ImageUrl; // Retorna a URL da imagem
            }

            return null; // Nenhum arquivo fornecido
        }

    }
}
