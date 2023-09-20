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

        IEnumerable<AppointmentModel> IAppointInterface.GetAllAppointments()
        {
            return _context.Appointments.ToList();
        }

        public AppointmentModel GetAppointmentById(int id)
        {
            return _context.Appointments.FirstOrDefault(a => a.AppointId == id);
        }

        public List<AppointmentModel> GetAppointmentsByUser(int userId)
        {
            // Getting all appointments for a user (whether they are a doctor or a patient)
            return _context.Appointments
                           .Where(a => a.Doctor.UserId == userId || a.Patient.UserId == userId)
                           .ToList();
        }


        public AppointmentModel AddAppointments(AppointmentModel appointment, int userId)
        {
            // Begin a new transaction
            var transaction = _context.Database.BeginTransaction();

            try
            {
                // Determine the UserType based on the userId (This is just a mock, implement the actual logic)
                UserTypeEnum currentUserType = DetermineUserType(userId);  // You need to implement this!

                Message msg = new Message();

                if (currentUserType == UserTypeEnum.Patient)
                {
                    msg.UserId = (int)UserTypeEnum.Patient;
                    msg.Content = appointment.PatientMsg;
                    msg.TimeSend = DateTime.Now;

                }
                else if (currentUserType == UserTypeEnum.Doctor)
                {
                    msg.UserId = (int)UserTypeEnum.Doctor;
                    msg.Content = appointment.DoctorMsg;
                    msg.TimeSend = DateTime.Now;
                }
                else
                {
                    throw new InvalidOperationException("Invalid user type.");
                }
                
                appointment.Messages = new List<Message> { msg };

                // Add the appointment to the database
                _context.Appointments.Add(appointment);

                _context.SaveChanges();

                // Update the message's AppointId
                //msg.AppointId = appointment.AppointId;

                _context.Message.Update(msg);
                var entries = _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();
                _context.SaveChanges();

                // Commit the transaction
                transaction.Commit();

                return appointment;
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message;
                // If there's any exception, roll back the transaction
                transaction.Rollback();
                throw new Exception("Error adding the appointment: " + innerExceptionMessage);
            }
            finally
            {
                // Ensure the transaction is disposed after use
                transaction.Dispose();
            }
        }

                private UserTypeEnum DetermineUserType(int userId)
                {
                    var userIsDoctor = _context.Doctors.Any(d => d.UserId == userId);
                    var userIsPatient = _context.Patients.Any(p => p.UserId == userId);

                    if (userIsDoctor)
                    {
                        return UserTypeEnum.Doctor;
                    }
    
                    if (userIsPatient)
                    {
                        return UserTypeEnum.Patient;
                    }

                    throw new InvalidOperationException("User type not recognized or user not found");
                }

        public AppointmentModel UpdateAppointments(AppointmentModel appointment, int userId)
        {
            var existingAppointment = _context.Appointments.Include(a => a.Messages)
                                        .FirstOrDefault(a => a.AppointId == appointment.AppointId);

            if (existingAppointment == null)
            {
                throw new ArgumentException("The specified appointment does not exist.");
            }

            UserTypeEnum currentUserType = DetermineUserType(userId);

            Message msg = new Message();

            // Setting message based on user type
            if (currentUserType == UserTypeEnum.Patient)
            {
                msg.UserId = (int)UserTypeEnum.Patient;
                msg.Content = appointment.PatientMsg;
                msg.TimeSend = DateTime.Now;
            }
            else if (currentUserType == UserTypeEnum.Doctor)
            {
                msg.UserId = (int)UserTypeEnum.Doctor;
                msg.Content = appointment.DoctorMsg;
                msg.TimeSend = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationException("Invalid user type.");
            }

            // Add the message to the existing appointment's messages
            existingAppointment.Messages.Add(msg);

            // Updating other appointment details
            existingAppointment.UpdateTime = DateTime.Now;
            existingAppointment.PatientMsg = appointment.PatientMsg;
            existingAppointment.DoctorMsg = appointment.DoctorMsg;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;  // Example update
                                                                                // ... Add any other fields from `appointment` to `existingAppointment` that you want to update...

            // Save changes
            _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);
            _context.SaveChanges();

            return existingAppointment;
        }

        public void DeleteAppointments(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointId == id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }
        }

   
    }
}