using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eConsultas_MVC.Models
{
    public class FileMV
    {
        public int ImgId { get; set; }
        public int UserId { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? imageFile { get; set; }
    }
}
