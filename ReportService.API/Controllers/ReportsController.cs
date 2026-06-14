using Microsoft.AspNetCore.Mvc;
using ReportService.API.DTOs;
using ReportService.API.Services;

namespace ReportService.API.Controllers
{
    [ApiController]
    [Route("reports")]
    public class ReportsController : ControllerBase
    {
        private readonly PdfReportService _pdfService;

        public ReportsController(PdfReportService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost("analytics/pdf")]
        public IActionResult GenerateAnalyticsPdf([FromBody] AnalyticsReportDto report)
        {
            var pdfBytes = _pdfService.GenerateAnalyticsReport(report);

            var fileName = $"Zvit_TO_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}