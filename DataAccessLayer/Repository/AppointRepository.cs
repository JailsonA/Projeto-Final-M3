using DataAccessLayer.Data;
using DataAccessLayer.Data.Enum;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DataAccessLayer.Interface;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Repository
{
    public class AppointRepository : IAppointInterface
    {
        private readonly ClinicaDbContext _context;

        private readonly ILogger<AppointRepository> _logger;
        private readonly IUserInterface _userRepository;
        private readonly IDecToken _decToken;



        public AppointRepository(ClinicaDbContext context)
        {

            _context = context;
        }

        /* Appointment Section */

        public AppointmentModel CreateAppointment(int doctorId, int patientId, string? patientMessage = null)
        {
            //transaction
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var doctor = _context.Doctors.Find(doctorId);
                var patient = _context.Patients.Find(patientId);

                if (doctor == null)
                {
                    throw new Exception("Médico não encontrado");
                }

                if (patient == null)
                {
                    throw new Exception("Paciente não encontrado");
                }

                var appointment = new AppointmentModel
                {
                    Doctor = doctor,
                    Patient = patient,
                    IsCompleted = false,
                    AppointmentDate = DateTime.Now,
                    Price = doctor.Fees,
                    PatientMsg = patientMessage
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                var message = new MessageModel
                {
                    UserId = patientId,
                    Message = patientMessage,
                    AppointId = appointment.AppointId,
                    TimeSend = DateTime.Now
                };

                _context.Message.Add(message);
                _context.SaveChanges();

                transaction.Commit();
                return appointment;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error add Appointment");
            }

        }

        public List<AppointmentModel> GetAppointmentId(int userId, int? appointmentId = null)
        {
            var query = _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Where(x => x.Doctor.UserId == userId || x.Patient.UserId == userId);

            if (appointmentId != null)
            {
                query = query.Where(x => x.AppointId == appointmentId);
            }

            return query.ToList();
        }

        public List<MessageModel> GetMessageByAppointId(int appointmentId, int userId)
        {
            var _user = _context.Users.Find(userId);
            var apoint = _context.Appointments.Find(appointmentId);

            if (apoint == null || (_user == null || (apoint.Doctor == null && apoint.Patient == null)))
            {
                return null;
            }

            if (apoint.Doctor != null && apoint.Doctor.UserId != userId && (apoint.Patient == null || apoint.Patient.UserId != userId))
            {
                return null;
            }

            var query = _context.Message
                .Include(x => x.User)
                .Where(x => x.AppointId == appointmentId);

            return query.ToList();
        }

        /* Message Section */
        public object AddMessage(int userId, int appointmentId, string message)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = _context.Users.Find(userId);
                var appointment = _context.Appointments.Find(appointmentId);

                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }

                if (appointment == null)
                {
                    throw new Exception("Consulta não encontrada");
                }

                if (appointment.IsCompleted == true)
                {
                    // Retorna todas as mensagens da consulta como uma lista
                    var messages = _context.Message
                        .Where(x => x.AppointId == appointmentId).ToList();

                    return messages;
                }

                var newMessage = new MessageModel
                {
                    UserId = user.UserId,
                    Message = message,
                    AppointId = appointment.AppointId,
                    TimeSend = DateTime.Now
                };

                _context.Message.Add(newMessage);
                _context.SaveChanges();

                if (user.UserType == UserTypeEnum.Doctor)
                {
                    appointment.DoctorMsg = message;
                }
                else if (user.UserType == UserTypeEnum.Patient)
                {
                    appointment.PatientMsg = message;
                }

                _context.Appointments.Update(appointment);
                _context.SaveChanges();

                transaction.Commit();
                return newMessage;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error add Message " + ex);
            }
        }

        //finishe appointment
        public object FinishAppointment(int userId, int appointmentId)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = _context.Users.Find(userId);
                var appointment = _context.Appointments.Find(appointmentId);

                if (user == null || user.UserType != UserTypeEnum.Doctor || appointment.Doctor.UserId != userId)
                {
                    throw new Exception("You dont have permition");
                }

                if (appointment == null)
                {
                    throw new Exception("Consulta não encontrada");
                }

                appointment.IsCompleted = !appointment.IsCompleted;
                _context.Appointments.Update(appointment);
                _context.SaveChanges();

                transaction.Commit();
                return appointment;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error finish Appointment");
            }
        }

        //public bool IsFileCopy(FileUser pdfFile, int userId)
        //{
        //    List<string> permExtensions = new List<string> { ".pdf", ".PDF" };
        //    string uploadDirectory = "pdf/Upload";
        //    ImgToDir imgToDir = new ImgToDir();
        //    string isUpload = imgToDir.CopyFile(pdfFile.imageFile, permExtensions, uploadDirectory, _context, userId);
        //    if (string.IsNullOrEmpty(isUpload)) return false;
        //    else return true;
        //}

        //download pdf file
        public string GetPdfFilePath(int appointId)
        {
            var appoint = _context.Appointments.Find(appointId);

            if (appoint != null)
            {
                var fileUser = _context.ImgUser.Find(int.Parse(appoint.PDFFile));

                if (fileUser != null)
                {
                    return Path.Combine("wwwroot", "pdf", "upload", fileUser.ImageUrl);
                }
            }

            return null;
        }
    }
}