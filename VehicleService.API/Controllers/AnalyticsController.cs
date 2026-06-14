using Microsoft.AspNetCore.Mvc;
using VehicleService.API.Data;

namespace VehicleService.API.Controllers
{
    [ApiController]
    [Route("analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard(DateTime? dateFrom, DateTime? dateTo)
        {
            var from = dateFrom ?? _context.Inspections.Min(x => x.SignDate);
            var to = dateTo ?? _context.Inspections.Max(x => x.SignDate);

            var query = _context.Inspections
                .Where(x => x.SignDate >= from && x.SignDate <= to);

            var totalInspections = query.Count();

            var complianceProtocols = query
                .Count(x => x.VehicleCompliance == "Так");

            var nonComplianceActs = query
                .Count(x => x.VehicleCompliance == "Ні");

            var internationalInspections = query
                .Count(x => x.IsInternational);

            var clientsCount = query
                .Select(x => x.Owner)
                .Distinct()
                .Count();

            var subjectToInspectionCount = query
                .Count(x => x.IsSubjectToInspection);

            var canceledCount = query
                .Count(x => x.IsCanceled);

            var ownersFirstDate = _context.Inspections
                .GroupBy(x => x.Owner)
                .Select(g => new
                {
                    Owner = g.Key,
                    FirstDate = g.Min(x => x.SignDate)
                });

            var newClientsCount = ownersFirstDate
                .Count(x => x.FirstDate >= from && x.FirstDate <= to);

            var byVehicleType = query
                .GroupBy(x => x.VehicleType)
                .Select(g => new
                {
                    name = g.Key,
                    count = g.Count()
                })
                .ToList();

            var byCategory = query
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    name = g.Key,
                    count = g.Count()
                })
                .ToList();

            var byStatus = query
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    name = g.Key,
                    count = g.Count()
                })
                .ToList();

            var byOwnerType = query
                .GroupBy(x => x.OwnerType)
                .Select(g => new
                {
                    name = g.Key,
                    count = g.Count()
                })
                .ToList();

            var byDate = query
                .GroupBy(x => x.SignDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    date = g.Key.ToString("dd.MM.yyyy"),
                    count = g.Count()
                })
                .ToList();

            return Ok(new
            {
                dateFrom = from.ToString("yyyy-MM-dd"),
                dateTo = to.ToString("yyyy-MM-dd"),

                totalInspections,
                complianceProtocols,
                nonComplianceActs,
                internationalInspections,
                clientsCount,
                newClientsCount,
                subjectToInspectionCount,
                canceledCount,

                byVehicleType,
                byCategory,
                byStatus,
                byOwnerType,
                byDate
            });
        }
    }
}