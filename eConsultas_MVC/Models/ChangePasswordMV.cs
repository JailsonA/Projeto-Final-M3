using System.ComponentModel.DataAnnotations;

namespace eConsultas_MVC.Models
{
    public class ChangePasswordMV
    {
        [Required(ErrorMessage = "insert old password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string OldPwd { get; set; }

        [Required(ErrorMessage = "insert new password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string NewPwd { get; set; }

        [Required(ErrorMessage = "Confirm new password")]
        [Compare("NewPwd", ErrorMessage = "Password not match")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string ConfirmPwd { get; set; }
    }
}
