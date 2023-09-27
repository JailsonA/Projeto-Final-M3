using DataAccessLayer.Data.Enum;
using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext()
        {

        }
        public ClinicaDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Obtenha a string de conexão do arquivo de configuração específico do DAL 
                var connectionString = "Data Source=DEV_PC;Initial Catalog=teste_eClinica;Trusted_Connection=True;TrustServerCertificate=True;"; //cn martelado
                //var connectionString = DalConfiguration.ConnectionString; // cn dinâmico
                optionsBuilder.UseSqlServer(connectionString);
            }
        }


        // Meus dbSets
        public DbSet<UserModel> Users { get; set; }
        public DbSet<FileUser> ImgUser { get; set; }
        public DbSet<AppointmentModel> Appointments { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<DoctorModel> Doctors { get; set; }
        public DbSet<MessageModel> Message { get; set; }
    }
}
