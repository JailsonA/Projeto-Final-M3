using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public interface IAppointInterface
    {

        IEnumerable<AppointmentModel> GetAllAppointments();
        AppointmentModel GetAppointmentById(int id);
        AppointmentModel AddAppointments(AppointmentModel appointment, int userId);
        AppointmentModel UpdateAppointments([FromBody] AppointmentModel appointment, int userId);
        void DeleteAppointments(int id);
        List<AppointmentModel> GetAppointmentsByUser(int userId);

    }

}