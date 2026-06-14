using Microsoft.AspNetCore.Mvc;
using VehicleService.API.Services;

namespace VehicleService.API.Controllers
{
    [ApiController]
    [Route("import")]
    public class ImportController : ControllerBase
    {
        private readonly ExcelImportService _service;

        public ImportController(ExcelImportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not uploaded");

            var count = await _service.ImportAsync(file);

            return Ok(new
            {
                message = "Excel imported successfully",
                imported = count
            });
        }
    }
}