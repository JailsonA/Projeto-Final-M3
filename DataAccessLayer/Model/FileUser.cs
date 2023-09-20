
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class FileUser
    {
        [Key]
        public int ImgId { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        [Required]
        public IFormFile? imageFile { get; set; }

    }
}
