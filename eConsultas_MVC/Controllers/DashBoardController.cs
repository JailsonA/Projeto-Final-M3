using eConsultas_MVC.Models;
using eConsultas_MVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eConsultas_MVC.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly HttpClient _httpClient;
        private ImgToDir _imgToDir;
        public DashBoardController(ImgToDir imgToDir)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5242/");
            _imgToDir = imgToDir;
        }

        /* Login */
        public IActionResult Index()
        {
            string _token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(_token))
            {
                return View();
            }

            return RedirectToAction("DashLand");
        }

        /* Dashboard */
        public async Task<IActionResult> DashLand()
        {
            string userImg = null;
            await Task.Delay(TimeSpan.FromSeconds(1)); // Atraso de 1 segundos

            string _token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(_token))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var user = await GetUser(_token);
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

                var getUser = JsonConvert.DeserializeObject<UserMV>(HttpContext.Session.GetString("User"));
                var img = GetAllUserFiles("/img/").Result;
                userImg = img.FirstOrDefault(x => x.UserId == getUser.UserId)?.ImageUrl;
                if (!string.IsNullOrEmpty(userImg))
                {
                    HttpContext.Session.SetString("UserImg", userImg);
                }

                var viewModel = new DashboardMV
                {
                    Appointments = await GetAppointments(_token)
                };

                return View(viewModel);
            }
        }

        /* Get user Logged Info */
        private async Task<UserMV> GetUser(string token)
        {
            string apiUrl = "Login/GetUserLogged";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    UserMV user = JsonConvert.DeserializeObject<UserMV>(responseContent);

                    return user;
                }
                else
                {
                    return new UserMV();
                }
            }
            catch (Exception ex)
            {
                return new UserMV();
            }
        }

        /* view create appointment*/
        [HttpGet]
        public async Task<IActionResult> CreateAppoint(string region, string city, string specialization)
        {
            string token = HttpContext.Session.GetString("Token");
            List<DoctorMV> doctors = await GetDoctorsBy(token, region, city, specialization);

            return View(doctors);
        }

        public IActionResult SendAppointment(int id)
        {
            //get doctor by id
            string token = HttpContext.Session.GetString("Token");
            var doctor = GetDoctors(token).Result.FirstOrDefault(x => x.UserId == id);

            return View(doctor);
        }

        /* Create Appointment just Patient can create a new appointment by add doctor and message*/
        [HttpPost]
        public async Task<IActionResult> AddApointment(int doctorId, string patientMessage)
        {
            string token = HttpContext.Session.GetString("Token");
            var user = JsonConvert.DeserializeObject<UserMV>(HttpContext.Session.GetString("User"));

            // Build the query parameters as a dictionary
            var queryParams = new Dictionary<string, string>
            {
                { "doctorId", doctorId.ToString() },
                { "patientMessage", patientMessage }
            };

            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            string apiUrl = $"Appointments/CreateAppointment?{queryString}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DashLand");
                }
                else
                {
                    return RedirectToAction("Erro");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Erro");
            }
        }

        /* Get all appointments by user id */
        private async Task<List<AppointmentMV>> GetAppointments(string token)
        {
            string apiUrl = "Appointments/GetAppointById";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var genericModels = JsonConvert.DeserializeObject<List<AppointmentMV>>(responseContent);
                    return genericModels;
                }
                else
                {
                    return new List<AppointmentMV>(null);
                }
            }
            catch (Exception ex)
            {
                return new List<AppointmentMV>(null);
            }
        }

        //finish appointment
        public async Task<IActionResult> FinishAppointment(int id)
        {
            string token = HttpContext.Session.GetString("Token");

            // Build the query parameters as a dictionary
            var queryParams = new Dictionary<string, string>
            {
                { "appointmentId", id.ToString() }
            };

            // Create a query string from the dictionary
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            // Construct the full API URL with the query string
            string apiUrl = $"Appointments/FinishAppointment?{queryString}";

            try
            {
                // Configure the HTTP client headers with authorization
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make the POST request with an empty content
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DashLand");
                }
                else
                {
                    return RedirectToAction("Erro");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return RedirectToAction("Erro");
            }
        }


        /* view Patient */
        public IActionResult CreatePatient()
        {
            return View();
        }

        /* Create Patient */
        [HttpPost]
        public IActionResult CreatePatient(CreatePatientMV createPatient)
        {
            string token = HttpContext.Session.GetString("Token");

            var content = new StringContent(JsonConvert.SerializeObject(createPatient), Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync("Users/Addpatient", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("DashLand");
            }
            else
            {
                return RedirectToAction("Erro");
            }
        }

        
        /* view Chat */
        [HttpGet]
        public async Task<IActionResult> Message(int id)
        {
            if (id != null)
            {
                string token = HttpContext.Session.GetString("Token");
                var appointment = GetAppointments(token).Result.FirstOrDefault(x => x.AppointId == id);

                if (appointment == null)
                {
                    return NotFound(null);
                }

                var messages = await GetMessages(token, id.ToString());
                var viewMessageModel = new MessageListsMV
                {
                    Messages = messages,
                    Appointments = appointment,
                    FilesImg = GetAllUserFiles("/img/").Result,
                    FilesPdf = GetAllUserFiles("/pdf/").Result
                };

                return View(viewMessageModel);
            }
            else
            {
                return null;
            }
        }

        /*Get all message by appointment Id*/
        public async Task<List<MessageMV>> GetMessages(string token, string appointmentId)
        {
            string apiUrl = $"Appointments/GetMessageByAppointId?appointmentId={appointmentId}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<MessageMV> messages = JsonConvert.DeserializeObject<List<MessageMV>>(responseContent);
                    return messages;
                }
                else
                {
                    return new List<MessageMV>(null);
                }
            }
            catch (Exception ex)
            {
                return new List<MessageMV>(null);
            }
        }


        /* Add message to appointment */
        [HttpPost]
        public async Task<IActionResult> AddMessage(string appointmentId, string message)
        {
            string token = HttpContext.Session.GetString("Token");
            var user = JsonConvert.DeserializeObject<UserMV>(HttpContext.Session.GetString("User"));

            var queryParams = new Dictionary<string, string>
            {
                { "appointmentId", appointmentId },
                { "message", message }
            };

            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            string apiUrl = $"Appointments/AddMessage?{queryString}";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Message", "DashBoard", new { id = appointmentId });
                }
                else
                {
                    return RedirectToAction("Erro");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Erro");
            }
        }

        /* Login */
        public async Task<IActionResult> Login(LoginMV login)
        {
            if (!ModelState.IsValid) return View(login);

            string apiUrl = "Login/Login";

            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Token");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    HttpContext.Session.SetString("Token", responseContent);
                    return RedirectToAction("DashLand");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            else
            {
                return RedirectToAction("Erro");
            }
        }

        //get all doctors
        public async Task<List<DoctorMV>> GetDoctors(string token)
        {
            string apiUrl = "Users/GetAllDoctor";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var genericModels = JsonConvert.DeserializeObject<List<DoctorMV>>(responseContent);
                    return genericModels;
                }
                else
                {
                    return new List<DoctorMV>(null);
                }
            }
            catch (Exception ex)
            {
                return new List<DoctorMV>(null);
            }
        }

        //get doctor by 
        public async Task<List<DoctorMV>> GetDoctorsBy(string token, string? region = null, string? city = null, string? specialization = null)
        {
            try
            {
                string apiUrl = "http://localhost:5242/Users/DoctorBy";

                // Create a query string with the optional parameters
                var queryParameters = new List<string>();
                if (!string.IsNullOrEmpty(region))
                {
                    queryParameters.Add($"region={region}");
                }
                if (!string.IsNullOrEmpty(city))
                {
                    queryParameters.Add($"city={city}");
                }
                if (!string.IsNullOrEmpty(specialization))
                {
                    queryParameters.Add($"specialization={specialization}");
                }

                if (queryParameters.Count > 0)
                {
                    apiUrl += "?" + string.Join("&", queryParameters);
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<DoctorMV> doctors = JsonConvert.DeserializeObject<List<DoctorMV>>(responseContent);
                    return doctors;
                }
                else
                {
                    return new List<DoctorMV>(null);
                }
            }
            catch (Exception ex)
            {
                return new List<DoctorMV>();
            }
        }


        /* view updates users info*/
        public IActionResult UpdateUser()
        {
            string token = HttpContext.Session.GetString("Token");
            var user = JsonConvert.DeserializeObject<UserMV>(HttpContext.Session.GetString("User"));
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }
            else
            {

                if (user.UserType == "Doctor")
                {
                    var viewModel = new UsersInfo
                    {
                        User = user,
                        Doctor = GetDoctors(token).Result.FirstOrDefault(x => x.UserId == user.UserId)

                    };
                    return View(viewModel);
                }
                else if (user.UserType == "Patient")
                {
                    var viewModel = new UsersInfo
                    {
                        User = user
                    };
                    return View(viewModel);
                }

            }
            return RedirectToAction("Index");
        }

        //update user Info
        [HttpPost]
        public IActionResult UpdateUser(DoctorMV doctor)
        {

            string token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(doctor), Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync("Users/UpdateDoctor", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UpdateUser");
            }
            else
            {
                return RedirectToAction("Erro");
            }
        }

        //change password
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordMV changePassword)
        {
            string token = HttpContext.Session.GetString("Token");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(changePassword), Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync("Login/ChangePwd", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UpdateUser");
            }
            else
            {
                return RedirectToAction("Erro");
            }
        }

        // save user image/pdf 
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file, int? appointId = null)
        {
            if (file == null)
            {
                return RedirectToAction("Erro");
            }

            string token = HttpContext.Session.GetString("Token");
            string fileUrl = null;

            try
            {
                List<string> imageExtensions = new List<string> { ".jpeg", ".png", ".jpg" };
                List<string> pdfExtensions = new List<string> { ".pdf" };

                string uploadDirectory = null;

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (imageExtensions.Contains(extension))
                {
                    uploadDirectory = "img/Upload";
                }
                else if (pdfExtensions.Contains(extension))
                {
                    uploadDirectory = "pdf/Upload";
                }
                else
                {
                    return RedirectToAction("Erro");
                }

                fileName = fileName + "_" + DateTime.Now.ToString("yymmfff") + extension;
                var filePath = Path.Combine("wwwroot", uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                fileUrl = filePath;

                if (fileUrl == null)
                {
                    return RedirectToAction("Erro");
                }

                var content = new StringContent($"fileUrl={fileUrl}", Encoding.UTF8, "application/x-www-form-urlencoded");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                HttpResponseMessage response;

                if (appointId == null)
                {
                    response = await _httpClient.PostAsync($"Users/addFile?fileUrl={fileUrl}", content);
                }
                else
                {
                    response = await _httpClient.PostAsync($"Users/addFile?fileUrl={fileUrl}&appointId={appointId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DashLand");
                }
                else
                {
                    return RedirectToAction("Erro");
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Erro");
            }
        }



        //get user logged image/pdf
        public async Task<List<FileMV>> GetAllUserFiles(string directory)
        {
            string token = HttpContext.Session.GetString("Token");
            string apiUrl = $"Users/GetImage";

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<FileMV> files = JsonConvert.DeserializeObject<List<FileMV>>(responseContent);
                    List<FileMV> returnFiles = new List<FileMV>();
                    files.ForEach(x => {
                        
                        if (x.ImageUrl.Contains("img"))
                        {
                            x.ImageUrl = x.ImageUrl.Replace("wwwroot\\", "~/");
                            x.ImageUrl = x.ImageUrl.Replace("\\", "/");

                            returnFiles.Add(x);
                        }
                    });


                    return returnFiles;
                }
                else
                {
                    return new List<FileMV>();
                }
            }
            catch (Exception ex)
            {
                return new List<FileMV>();
            }
        }

        public IActionResult Erro()
        {
            return View();
        }

        //logout clear session
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Home");
        }
    }
}
