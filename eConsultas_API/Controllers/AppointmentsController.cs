using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Data.Enum;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Filters;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;

namespace eConsultas_API.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointInterface _appointmentRepository;

        private readonly ILogger<AppointRepository> _logger;
        private readonly IDecToken _decToken;
        private readonly ClinicaDbContext _context;

        public AppointmentsController(ILogger<AppointRepository> logger, IAppointInterface appointmentRepository, IDecToken decToken, ClinicaDbContext context)
        {
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _decToken = decToken;
            _context = context;
        }

        [HttpPost]
        [UserAcess]
        public IActionResult CreateAppointment(string doctorId, string patientMessage, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var loggedUser = _decToken.GetLoggedUser(token);

                if (loggedUser == null) return NotFound("Invalid user or password");

                if (loggedUser.UserType != "Patient") return BadRequest("Invalid user");

                var appointment = _appointmentRepository.CreateAppointment(Convert.ToInt32(doctorId), loggedUser.UserId, patientMessage);

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [UserAcess]
        public IActionResult GetAppointById([FromHeader(Name = "Authorization")] string authorizationHeader, int? apointId = null)
        {
            try
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var loggedUser = _decToken.GetLoggedUser(token);

                var appointment = _appointmentRepository.GetAppointmentId(loggedUser.UserId, apointId);

                if (appointment == null) return NotFound("Appointment not found");

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //add message endpoint
        [HttpPost]
        [UserAcess]
        public IActionResult AddMessage(string appointmentId, string message, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var loggedUser = _decToken.GetLoggedUser(token);

                if (loggedUser == null) return NotFound("Invalid user or password");

                var messageModel = _appointmentRepository.AddMessage(loggedUser.UserId, Convert.ToInt32(appointmentId), message);

                return Ok(messageModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [UserAcess]
        public IActionResult GetMessageByAppointId(string appointmentId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var loggedUser = _decToken.GetLoggedUser(token);

                if (loggedUser == null) return NotFound("Invalid user or password");

                var messageModel = _appointmentRepository.GetMessageByAppointId(Convert.ToInt32(appointmentId), loggedUser.UserId);

                return Ok(messageModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [UserAcess]
        public IActionResult FinishAppointment(string appointmentId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var loggedUser = _decToken.GetLoggedUser(token);

                if (loggedUser == null) return NotFound("Invalid user or password");

                var messageModel = _appointmentRepository.FinishAppointment(loggedUser.UserId, Convert.ToInt32(appointmentId));

                return Ok(messageModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
