using DataAccessLayer.Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    [JsonObject]
    public class DoctorModel : UserModel
    {
        public string? Especialization { get; set; }
        public float? Fees { get; set; }
        public string? AdInfo { get; set; }

        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
    }
}
