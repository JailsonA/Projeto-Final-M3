using System.ComponentModel.DataAnnotations;

namespace eConsultas_MVC.Models
{
    public class AppointmentMV
    {
        public int AppointId { get; set; }
        public DoctorMV Doctor { get; set; }
        public PatientMV Patient { get; set; }
        public string? PDFFile { get; set; } // Nome do paciente
        public string PatientMsg { get; set; } // Mensagem do paciente
        public string? DoctorMsg { get; set; } // Mensagem do médico
        public float? Price { get; set; } // Preço da consulta
        public DateTime UpdateTime { get; set; } // Data e hora da consulta
        public DateTime AppointmentDate { get; set; }
        public string? info { get; set; }
        public bool IsCompleted { get; set; } // Indica se a consulta foi concluída
    }
}
