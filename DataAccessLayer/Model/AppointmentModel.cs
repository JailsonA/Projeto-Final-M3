
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
        public string? PDFFile { get; set; } // Nome do paciente
        public string PatientMsg { get; set; } // Mensagem do paciente
        public string DoctorMsg { get; set; } // Mensagem do médico
        public DateTime UpdateTime { get; set; } // Data e hora da consulta
        public DateTime AppointmentDate { get; set; }
        public string info { get; set; }
        public bool IsCompleted { get; set; } // Indica se a consulta foi concluída
        public List<Message> Messages { get; set; }
    }
}
