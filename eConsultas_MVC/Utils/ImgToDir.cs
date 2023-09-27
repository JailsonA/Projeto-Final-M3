using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eConsultas_MVC.Utils
{
    public class ImgToDir
    {
        public string CopyFile(IFormFile imageFile, List<string> permExtensions, string uploadDirectory)
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

                return Path.Combine(uploadDirectory, fileName); // Retorna a URL da imagem
            }

            return null; // Nenhum arquivo fornecido
        }

        public string CopyPdf(IFormFile imageFile, List<string> permExtensions, string uploadDirectory)
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



                return Path.Combine(uploadDirectory, fileName); // Retorna a URL da imagem
            }

            return null; // Nenhum arquivo fornecido
        }

    }
}
