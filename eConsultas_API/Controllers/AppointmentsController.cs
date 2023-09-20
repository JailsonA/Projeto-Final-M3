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

        [HttpGet]
        public IActionResult GetAllAppointments()
        {
            return Ok(_appointmentRepository.GetAllAppointments());
        }

        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(int id)
        {
            var appointment = _appointmentRepository.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            return Ok(appointment);
        }

        [UserAcess]  // Ensure the user is authorized
        [HttpPost]
        public ActionResult<AppointmentModel> AddAppointment([FromBody] AppointmentModel appointment, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract the token from the authorization header.
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Check if token is not null or empty.
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token");
            }

            // Get the logged in user using the token.
            var loggedUser = _decToken.GetLoggedUser(token);

            // If the user is not found, return a NotFound error.
            if (loggedUser == null)
            {
                return NotFound("Invalid user or password");
            }

            try
            {
                var result = _appointmentRepository.AddAppointments(appointment, loggedUser.UserId);
                return Ok(new { message = "Appointment added successfully!", data = result, });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [UserAcess]  // Ensure the user is authorized
        [HttpGet]
        public IActionResult GetAppointmentsByUser(int userId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // The same token check as before...
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token");
            }

            var loggedUser = _decToken.GetLoggedUser(token);

            if (loggedUser == null)
            {
                return NotFound("Invalid user or password");
            }

            if (loggedUser.UserId != userId)
            {
                return Unauthorized("You are not authorized to view these appointments.");
            }

            var appointments = _appointmentRepository.GetAppointmentsByUser(userId);
            return Ok(appointments);
        }



        [HttpPut]
        public IActionResult UpdateAppointments(AppointmentModel appointment, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            try
            {
                var updatedAppointment = _appointmentRepository.UpdateAppointments(appointment, userId);
                return Ok(new
                {
                    message = "Appointments updated successfully",
                    patient = updatedAppointment
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteAppointments(int id)
        {
            _appointmentRepository.DeleteAppointments(id);
            return NoContent();  // Successful DELETE typically returns NoContent
        }
    }
}
