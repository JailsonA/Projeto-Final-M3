namespace eConsultas_MVC.Models
{
    public class MessageListsMV
    {
        public List<MessageMV> Messages { get; set; }
        public AppointmentMV Appointments { get; set; }
        public List<FileMV> FilesImg { get; set; }
        public List<FileMV> FilesPdf { get; set; }
    }
}
