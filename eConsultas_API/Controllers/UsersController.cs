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
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Newtonsoft.Json.Linq;

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

        /* Doctor Section */
        [HttpPost]
        [PrivilegeUser("Doctor")]
        public IActionResult UpdateDoctor([FromHeader(Name = "Authorization")] string authorizationHeader, [FromBody] DoctorUpdateInfo doctor)
        {

            // Verifique se o token é válido e obtenha o usuário logado.
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var userLogged = _decToken.GetLoggedUser(token);
            if (userLogged == null)
            {
                return BadRequest("Token inválido ou usuário não encontrado.");
            }

            // Verifique se o usuário a ser atualizado existe.
            var userToUpdate = _userRepository.GetUserByIdGen<DoctorModel>(userLogged.UserId);
            if (userToUpdate == null)
            {
                return BadRequest("Usuário a ser atualizado não encontrado.");
            }

            // Atualize as propriedades do usuário com os valores do objeto "doctor".
            if (doctor.Especialization != null) userToUpdate.Especialization = doctor.Especialization;
            if (doctor.Fees != null) userToUpdate.Fees = doctor.Fees;
            if (doctor.AdInfo != null) userToUpdate.AdInfo = doctor.AdInfo;
            if (doctor.Region != null) userToUpdate.Region = doctor.Region;
            if (doctor.City != null) userToUpdate.City = doctor.City;
            if (doctor.Address != null) userToUpdate.Address = doctor.Address;

            if (userToUpdate != null)
            {
                string msg = "Update doctor" + doctor.DoctorUserId;
                if (!loggers(msg, authorizationHeader)) return BadRequest("Invalid token");
                return Ok(_userRepository.UpdateUserGen<DoctorModel>(userToUpdate));
            }
            else
                return NotFound();
        }



        /* Patient Section */
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

        [HttpGet]
        [UserAcess]
        public IActionResult GetAllDoctor([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            return Ok(_userRepository.GetAllUsersGen<DoctorModel>());
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

        [HttpPost]
        [UserAcess]
        public IActionResult addFile(string fileUrl, [FromHeader(Name = "Authorization")] string authorizationHeader)
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
            FileUser image = new FileUser();
            image.ImageUrl = fileUrl;
            image.UserId = userId;
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

        //receid token get user logged and return image url
        [HttpGet]
        [UserAcess]
        public IActionResult GetImage([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
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

            var imageFile = _userRepository.GetImage();

            if (imageFile != null)
            {
                return Ok(imageFile);
            }
            else
            {
                return BadRequest("Error sending image");
            }
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
