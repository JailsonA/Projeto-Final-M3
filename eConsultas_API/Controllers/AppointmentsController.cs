using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Data.Enum;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Filters;
using DataAccessLayer.Interface;

namespace eConsultas_API.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointInterface _appointmentRepository;

        private readonly ILogger<AppointRepository> _logger;
        private readonly IDecToken _decToken;

        public AppointmentsController(ILogger<AppointRepository> logger, IAppointInterface appointmentRepository, IDecToken decToken)
        {

            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _decToken = decToken;
        }


    }
}
