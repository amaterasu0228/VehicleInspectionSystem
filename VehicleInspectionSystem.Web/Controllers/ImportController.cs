using Microsoft.AspNetCore.Mvc;

namespace VehicleInspectionSystem.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly HttpClient _httpClient;

        public ImportController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Оберіть файл для імпорту.";
                return View();
            }

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();

            var fileContent = new StreamContent(stream);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("https://localhost:7065/import", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Inspections");
            }

            ViewBag.Message = "Помилка імпорту файлу.";
            return View();
        }
    }
}