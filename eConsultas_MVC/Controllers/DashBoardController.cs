using Microsoft.AspNetCore.Mvc;

namespace eConsultas_MVC.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DashLand()
        {
            return View();
        }

        public IActionResult CreateAppoint()
        {
            return View();
        }
    }
}
