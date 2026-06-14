using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Services;

namespace VehicleInspectionSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _api;

        public HomeController(ApiService api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Inspections");
        }
    }
}