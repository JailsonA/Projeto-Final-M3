using eConsultas_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eConsultas_MVC.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashBoardController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5242/");

        }

        public IActionResult Index()
        {
            string _token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(_token))
            {
                return View();
            }

            return RedirectToAction("DashLand");
        }

        public async Task<IActionResult> DashLand()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // Atraso de 2 segundos
            string _token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(_token))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var user = await GetUser(_token);
                // save user in session
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));


                var viewModel = new DashboardMV
                {
                    Appointments = await GetAppointments(_token)
                };

                return View(viewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> CreateAppoint(string region, string city, string specialization)
        {
            string token = HttpContext.Session.GetString("Token");

            // Call your API method to retrieve doctors with the query parameters
            List<DoctorMV> doctors = await GetDoctorsBy(token, region, city, specialization);

            return View(doctors);
        }


        public IActionResult CreatePatient()
        {
            return View();
        }

        public IActionResult SendAppointment(int id)
        {
            //get doctor by id
            string token = HttpContext.Session.GetString("Token");
            var doctor = GetDoctors(token).Result.FirstOrDefault(x => x.UserId == id);

            return View(doctor);
        }

        //Endpoin addApointment
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

            // Create a query string from the dictionary
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            // Construct the full API URL with the query string
            string apiUrl = $"Appointments/CreateAppointment?{queryString}";

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


        [HttpGet]
        public async Task<IActionResult> Message(int id)
        {
            if (id != null)
            {
                // Get appointment by id
                string token = HttpContext.Session.GetString("Token");
                var appointment = GetAppointments(token).Result.FirstOrDefault(x => x.AppointId == id);

                if (appointment == null)
                {
                    // Handle the case where the appointment with the given ID was not found
                    return NotFound();
                }

                var messages = await GetMessages(token, id.ToString());
                var viewMessageModel = new MessageListsMV
                {
                    Messages = messages,// You need to implement this method
                    Appointments = appointment
                };

                return View(viewMessageModel);
            }
            else
            {
                return null;
            }
        }

        //get messages by appointment id
        public async Task<List<MessageMV>> GetMessages(string token, string appointmentId)
        {
            string apiUrl = $"Appointments/GetMessageByAppointId?appointmentId={appointmentId}";

            try
            {
                // Configure the authorization header with the token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make the API call
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<MessageMV> messages = JsonConvert.DeserializeObject<List<MessageMV>>(responseContent);
                    return messages;
                }
                else
                {
                    // Handle the error as needed, e.g., return an empty list or throw an exception
                    return new List<MessageMV>();
                }
            }
            catch (Exception ex)
            {
                // Handle the error as needed, e.g., return an empty list or throw an exception
                return new List<MessageMV>();
            }
        }


        //Endpoin addMessage
        [HttpPost]
        public async Task<IActionResult> AddMessage(string appointmentId, string message)
        {
            string token = HttpContext.Session.GetString("Token");
            var user = JsonConvert.DeserializeObject<UserMV>(HttpContext.Session.GetString("User"));

            // Build the query parameters as a dictionary
            var queryParams = new Dictionary<string, string>
            {
                { "appointmentId", appointmentId },
                { "message", message }
            };

            // Create a query string from the dictionary
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;

            // Construct the full API URL with the query string
            string apiUrl = $"Appointments/AddMessage?{queryString}";

            try
            {
                // Configure the HTTP client headers with authorization
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make the POST request with an empty content
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Message", "DashBoard", new { id = appointmentId });
                }
                else
                {
                    return RedirectToAction("Erro", "Home");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return RedirectToAction("Erro");
            }
        }

        public async Task<IActionResult> Login(LoginMV login)
        {
            if (!ModelState.IsValid) return View(login);

            string apiUrl = "Login/Login";

            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Limpe a sessão
                HttpContext.Session.Clear();
                //clear session Token
                HttpContext.Session.Remove("Token");
                string responseContent = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("Token", responseContent);
                // Redirecione para a view DashLand com os dados carregados
                return RedirectToAction("DashLand");
            }
            else
            {
                return RedirectToAction("Erro");
            }
        }

        // get User logged passing token
        private async Task<UserMV> GetUser(string token)
        {
            string apiUrl = "Login/GetUserLogged";

            try
            {
                // Configure o cabeçalho de autorização com o token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Faça a chamada à API
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

        private async Task<List<AppointmentMV>> GetAppointments(string token)
        {
            string apiUrl = "Appointments/GetAppointById";

            try
            {
                // Configure o cabeçalho de autorização com o token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Faça a chamada à API
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var genericModels = JsonConvert.DeserializeObject<List<AppointmentMV>>(responseContent);
                    return genericModels;
                }
                else
                {
                    // Trate o erro de acordo com suas necessidades
                    // Pode ser lançada uma exceção ou retornada uma lista vazia, dependendo do caso.
                    return new List<AppointmentMV>();
                }
            }
            catch (Exception ex)
            {
                // Trate o erro de acordo com suas necessidades
                // Pode ser lançada uma exceção ou retornada uma lista vazia, dependendo do caso.
                return new List<AppointmentMV>();
            }
        }

        //get all doctors
        public async Task<List<DoctorMV>> GetDoctors(string token)
        {
            string apiUrl = "Users/GetAllDoctor";

            try
            {
                // Configure o cabeçalho de autorização com o token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Faça a chamada à API
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var genericModels = JsonConvert.DeserializeObject<List<DoctorMV>>(responseContent);
                    return genericModels;
                }
                else
                {
                    // Trate o erro de acordo com suas necessidades
                    // Pode ser lançada uma exceção ou retornada uma lista vazia, dependendo do caso.
                    return new List<DoctorMV>();
                }
            }
            catch (Exception ex)
            {
                // Trate o erro de acordo com suas necessidades
                // Pode ser lançada uma exceção ou retornada uma lista vazia, dependendo do caso.
                return new List<DoctorMV>();
            }
        }

        //get doctor by id
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

                // Configure the HttpClient with the token
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Make the GET request
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        List<DoctorMV> doctors = JsonConvert.DeserializeObject<List<DoctorMV>>(responseContent);
                        return doctors;
                    }
                    else
                    {
                        // Handle the error as needed (e.g., return an empty list or throw an exception)
                        return new List<DoctorMV>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed (e.g., log the error or return an empty list)
                return new List<DoctorMV>();
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

        // send pdf to endpoint Users/addPdf sendinding IFormFile and appointmentId
        [HttpPost]
        public async Task<IActionResult> AddPdf(IFormFile file, int appoint)
        {
            string token = HttpContext.Session.GetString("Token");

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Adicione o arquivo à solicitação
                    content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                    // Configure os cabeçalhos HTTP com autorização
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Faça a solicitação PUT com o conteúdo do arquivo
                    HttpResponseMessage response = await _httpClient.PutAsync($"Users/addPdf?appoint={appoint}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Message", "DashBoard", new { id = appoint });
                    }
                    else
                    {
                        return RedirectToAction("Erro");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return RedirectToAction("Erro");
            }
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
