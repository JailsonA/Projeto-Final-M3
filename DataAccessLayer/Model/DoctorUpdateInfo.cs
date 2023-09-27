using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class DoctorUpdateInfo
    {
        public int DoctorUserId { get; set; }
        public string? Especialization { get; set; }
        public float? Fees { get; set; }
        public string? AdInfo { get; set; }

        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
    }
}
