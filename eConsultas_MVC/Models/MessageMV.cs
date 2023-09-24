namespace eConsultas_MVC.Models
{
    public class MessageMV
    {
        public int UserId { get; set; }
        public UserMV User { get; set; }
        public string Message { get; set; }
        public int AppointId { get; set; }
        public DateTime TimeSend { get; set; }
    }
}
