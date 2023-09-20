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


    }
}