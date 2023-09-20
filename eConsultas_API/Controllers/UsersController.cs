using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Model;
using Microsoft.Extensions.Logging; // Adicionado para ILogger
using DataAccessLayer.Interface;
using DataAccessLayer.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using DataAccessLayer.Data.Enum;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using Microsoft.AspNetCore.Identity;

namespace eConsultas_API.Controllers
{

    [ApiController]
    [Route("[controller]/[Action]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserInterface _userRepository;
        private readonly IDecToken _decToken;

        public UsersController(ILogger<UsersController> logger, IUserInterface userRepository, IDecToken decToken)
        {
            _logger = logger;
            _userRepository = userRepository;
            _decToken = decToken;
        }

        /* Admin Section */

        [HttpGet]
        [PrivilegeUser("Admin")]
        public IActionResult GetDoctorById(int doctorId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            return Ok(_userRepository.GetUserByIdGen<PatientModel>(doctorId));
        }

        [HttpGet]
        public IActionResult DoctorBy(string? region = null, string? city = null, string? Specialization = null)
        {
            return Ok(_userRepository.GetDoctorBy(region, city, Specialization));
        }

        [HttpPost]
        public IActionResult AddDoctor([FromBody] DoctorModel doctor) //, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                //string msg = "Try to Add Doctor" + doctor.FullName;
                //if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
                if (ModelState.IsValid)
                {
                    doctor.UserType = UserTypeEnum.Doctor;
                    return Ok(_userRepository.AddUserGen(doctor));
                }
                else
                {
                    return BadRequest("Bad request");
                }
            }
            catch (ArgumentException ex)
            {
                // Capturando a exceção de email duplicado e retornando uma resposta de erro apropriada
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [PrivilegeUser("Admin")]
        public IActionResult DeleteDoctor(int doctorId, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            var user = _userRepository.GetUserByIdGen<DoctorModel>(doctorId);
            string msg = "Delet doctor: " + doctorId;
            if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_userRepository.DeleteUserGen<DoctorModel>(user.UserId));
        }

        [HttpGet]
        [PrivilegeUser("Admin")]
        public IActionResult GetDoctorByEmail(string email)
        {
            return Ok(_userRepository.GetUserByEmailGen<DoctorModel>(email));
        }

        [HttpGet]
        [PrivilegeUser("Admin")]
        public IActionResult GetAllPatient()
        {
            return Ok(_userRepository.GetAllUsersGen<PatientModel>());
        }

        [HttpDelete]
        [PrivilegeUser("Admin")]
        public IActionResult DeletePatient(int id, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                var user = _userRepository.GetUserByIdGen<PatientModel>(id);
                if (user == null)
                {
                    return NotFound("Patient not found.");
                }
                string msg = "Delet Patient: " + user.UserId;
                if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
                var result = _userRepository.DeleteUserGen<PatientModel>(user.UserId);
                if (result)
                {

                    return Ok("Patient deleted successfully.");
                }
                return BadRequest("Error deleting patient.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [PrivilegeUser("Admin")]
        public IActionResult GetPatientByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            var patient = _userRepository.GetUserByEmailGen<PatientModel>(email);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }

            return Ok(patient);
        }

        [HttpGet]
        [PrivilegeUser("Admin")]
        public IActionResult GetAllDoctor([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            return Ok(_userRepository.GetAllUsersGen<DoctorModel>());
        }


        /* Doctor Section */
        [HttpPut]
        [PrivilegeUser("Doctor")]
        public IActionResult UpdateDoctor([FromBody] DoctorModel doctor, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if (ModelState.IsValid)
            {
                string msg = "Delet doctor" + doctor.UserId;
                if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
                return Ok(_userRepository.UpdateUserGen<DoctorModel>(doctor));
            }
            else
                return NotFound();
        }


        /* Patient Section user acess*/



        [HttpGet]
        [UserAcess]
        public IActionResult GetPatientById(int patientId)
        {
            return Ok(_userRepository.GetUserByIdGen<PatientModel>(patientId));
        }

        [PrivilegeUser("Patient")]
        [HttpPost]
        public IActionResult UpdatePatient(PatientModel patient, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            try
            {
                var updatedPatient = _userRepository.UpdateUserGen<PatientModel>(patient);
                string msg = "Update Perfil: " + patient.UserId;
                if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
                return Ok(new
                {
                    message = "Patient updated successfully",
                    patient = updatedPatient
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        [UserAcess]
        public IActionResult addImage(ImageUser image, [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedUser = _decToken.GetLoggedUser(token);

            if (loggedUser == null)
            {
                return NotFound("Invalid user or password");
            }
            int userId = loggedUser.UserId;
            bool isSend = _userRepository.IsFileCopy(image, userId);
            if (isSend)
            {
                return Ok("Image send successfully");
            }
            else
            {
                return BadRequest("Error sending image");
            }
        }

        /* metodos aberto sem restrinção*/
        [HttpPost]
        public IActionResult AddPatient(PatientModel patient)
        {
            if (ModelState.IsValid)
            {
                patient.UserType = UserTypeEnum.Patient;
                var addedPatient = _userRepository.AddUserGen(patient);
                return Ok(new
                {
                    message = "Patient added successfully",
                    patient = addedPatient
                });
            }
            else
            {
                return BadRequest("Invalid model state");
            }
        }



        //aberto para teste depois apagar o metodo
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            ImageUser image = new ImageUser();
            image.imageFile = file;
            int userId = 1;
            bool isSend = _userRepository.IsFileCopy(image, userId);
            return Ok(new { message = "Arquivo enviado com sucesso!" });
        }


        /*testar metodo logger*/
        private bool loggers(string msg, string authorizationHeader)
        {
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                // Extrai o token do cabeçalho
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    var user = _decToken.GetLoggedUser(token);
                    if (user == null) return false;

                    string userId = user.UserId.ToString();
                    string obs = msg;
                    _logger.LogInformation("UserId: {UserId} acess {Obs} ", userId, obs);
                    return true;
                }
            }
            return false;
        }

    }


}
