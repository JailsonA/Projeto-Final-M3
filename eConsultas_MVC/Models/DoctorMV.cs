namespace eConsultas_MVC.Models
{
    public class DoctorMV : UserMV
    {
        public string Especialization { get; set; }
        public string Fees { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
    }
}
