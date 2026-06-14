using DocumentService.API.Data;
using DocumentService.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.API.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var documents = _context.Documents
                .OrderByDescending(x => x.UploadedAt)
                .ToList();

            return Ok(documents);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
    IFormFile file,
    [FromForm] string title,
    [FromForm] string category)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не обрано.");

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Storage");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new DocumentFile
            {
                Title = title,
                Category = category,
                FileName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                UploadedAt = DateTime.Now
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(document);
        }

        [HttpGet("download/{id}")]
        public IActionResult Download(int id)
        {
            var document = _context.Documents.FirstOrDefault(x => x.Id == id);

            if (document == null)
                return NotFound();

            if (!System.IO.File.Exists(document.FilePath))
                return NotFound("Файл не знайдено на диску.");

            var fileBytes = System.IO.File.ReadAllBytes(document.FilePath);

            return File(fileBytes, document.ContentType, document.FileName);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var document = _context.Documents.FirstOrDefault(x => x.Id == id);

            if (document == null)
                return NotFound();

            if (System.IO.File.Exists(document.FilePath))
                System.IO.File.Delete(document.FilePath);

            _context.Documents.Remove(document);
            _context.SaveChanges();

            return Ok();
        }
    }
}