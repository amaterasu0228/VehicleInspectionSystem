using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Services;

namespace VehicleInspectionSystem.Web.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ApiService _api;

        public DocumentsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _api.GetDocumentsAsync();

            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(
    IFormFile file,
    string title,
    string category)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Файл не обрано.";
                return RedirectToAction(nameof(Index));
            }

            var success = await _api.UploadDocumentAsync(
                file,
                title,
                category);

            if (!success)
                TempData["Error"] = "Не вдалося завантажити документ.";
            else
                TempData["Success"] = "Документ успішно завантажено.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download(int id)
        {
            var url = _api.GetDownloadUrl(id);

            return Redirect(url);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteDocumentAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}