
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class AppointmentModel
    {
        [Key]
        public int AppointId { get; set; }
        public DoctorModel Doctor { get; set; }
        public PatientModel Patient { get; set; }
        public string? PDFFile { get; set; }
        public string PatientMsg { get; set; }
        public string? DoctorMsg { get; set; }
        public float? Price { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? info { get; set; }
        public bool IsCompleted { get; set; }

    }
}
