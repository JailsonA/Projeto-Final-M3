namespace eConsultas_MVC.Models
{
    public class UserMV
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string TokenKey { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
