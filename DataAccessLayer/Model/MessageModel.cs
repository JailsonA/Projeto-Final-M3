using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using DataAccessLayer.Model;

namespace DataAccessLayer.Model
{


    public class MessageModel
    {
        [Key]
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public string Message { get; set; }
        public int AppointId { get; set; }
        public DateTime TimeSend { get; set; }
    }



}


