namespace eConsultas_MVC.Models
{
    public class UsersInfo
    {
        public DoctorMV Doctor { get; set; }
        public PatientMV Patient { get; set; }
        public UserMV User { get; set; }
        public List<FileMV> Files { get; set; }
        public ChangePasswordMV ChangePassword { get; set; }

    }
}
