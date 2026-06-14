using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleService.API.Data;
using VehicleService.API.Models;

namespace VehicleService.API.Controllers
{
    [ApiController]
    [Route("inspections")]
    public class InspectionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InspectionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _context.Inspections.ToList();
            return Ok(data);
        }
        [HttpGet("search")]
        public IActionResult Search(string plate)
        {
            var result = _context.Inspections
                .Where(x => x.PlateNumber.Contains(plate))
                .ToList();

            return Ok(result);
        }
        [HttpGet("status")]
        public IActionResult ByStatus(string status)
        {
            var result = _context.Inspections
                .Where(x => x.Status == status)
                .ToList();

            return Ok(result);
        }
        [HttpGet("by-date")]
        public IActionResult ByDate(DateTime from, DateTime to)
        {
            var result = _context.Inspections
                .Where(x => x.SignDate >= from && x.SignDate <= to)
                .ToList();

            return Ok(result);
        }
        [HttpGet("filter")]
        public IActionResult Filter(
    string? plate,
    string? vin,
    string? owner,
    string? brand,
    string? model,
    DateTime? dateFrom,
    DateTime? dateTo,
    string? sortBy,
    string? sortDirection)
        {
            var query = _context.Inspections.AsQueryable();

            if (!string.IsNullOrWhiteSpace(plate))
                query = query.Where(x => x.PlateNumber.Contains(plate));

            if (!string.IsNullOrWhiteSpace(vin))
                query = query.Where(x => x.VIN.Contains(vin));

            if (!string.IsNullOrWhiteSpace(owner))
                query = query.Where(x => x.Owner.Contains(owner));

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(x => x.Brand.Contains(brand));

            if (!string.IsNullOrWhiteSpace(model))
                query = query.Where(x => x.Model.Contains(model));

            if (dateFrom.HasValue)
                query = query.Where(x => x.SignDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.SignDate <= dateTo.Value);

            bool desc = sortDirection == "desc";

            query = sortBy switch
            {
                "date" => desc ? query.OrderByDescending(x => x.SignDate) : query.OrderBy(x => x.SignDate),
                "brand" => desc ? query.OrderByDescending(x => x.Brand) : query.OrderBy(x => x.Brand),
                "model" => desc ? query.OrderByDescending(x => x.Model) : query.OrderBy(x => x.Model),
                "owner" => desc ? query.OrderByDescending(x => x.Owner) : query.OrderBy(x => x.Owner),
                "plate" => desc ? query.OrderByDescending(x => x.PlateNumber) : query.OrderBy(x => x.PlateNumber),
                _ => query.OrderBy(x => x.Id)
            };

            return Ok(query.ToList());
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var inspection = _context.Inspections.FirstOrDefault(x => x.Id == id);

            if (inspection == null)
                return NotFound();

            return Ok(inspection);
        }

        [HttpPost]
        public IActionResult Create([FromBody] VehicleInspection inspection)
        {
            _context.Inspections.Add(inspection);
            _context.SaveChanges();

            return Ok(inspection);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] VehicleInspection updated)
        {
            var inspection = _context.Inspections.FirstOrDefault(x => x.Id == id);

            if (inspection == null)
                return NotFound();

            inspection.VehicleCompliance = updated.VehicleCompliance;
            inspection.DocumentNumber = updated.DocumentNumber;
            inspection.BlankNumber = updated.BlankNumber;
            inspection.Status = updated.Status;

            inspection.SignDate = updated.SignDate;
            inspection.SignTime = updated.SignTime;

            inspection.VehicleType = updated.VehicleType;
            inspection.Brand = updated.Brand;
            inspection.Model = updated.Model;
            inspection.Category = updated.Category;

            inspection.PlateNumber = updated.PlateNumber;
            inspection.VIN = updated.VIN;

            inspection.Owner = updated.Owner;
            inspection.OwnerPhone = updated.OwnerPhone;
            inspection.OwnerType = updated.OwnerType;

            inspection.IsSubjectToInspection = updated.IsSubjectToInspection;
            inspection.InspectionPeriod = updated.InspectionPeriod;

            inspection.NextInspectionDate = updated.NextInspectionDate;

            inspection.IsInternational = updated.IsInternational;
            inspection.IsCanceled = updated.IsCanceled;

            _context.SaveChanges();

            return Ok(inspection);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var inspection = _context.Inspections.FirstOrDefault(x => x.Id == id);

            if (inspection == null)
                return NotFound();

            _context.Inspections.Remove(inspection);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("expiring")]
        public IActionResult GetExpiringInspections(int days = 30)
        {
            var today = DateTime.Today;
            var limitDate = today.AddDays(days);

            var result = _context.Inspections
                .Where(x => x.NextInspectionDate != null &&
                            x.NextInspectionDate >= today &&
                            x.NextInspectionDate <= limitDate)
                .OrderBy(x => x.NextInspectionDate)
                .Select(x => new
                {
                    x.Id,
                    x.DocumentNumber,
                    x.VehicleCompliance,
                    x.Status,
                    x.VehicleType,
                    x.Brand,
                    x.Model,
                    x.Category,
                    x.PlateNumber,
                    x.VIN,
                    x.Owner,
                    x.OwnerType,
                    x.NextInspectionDate,

                    DaysLeft = EF.Functions.DateDiffDay(today, x.NextInspectionDate!.Value),

                    ReminderType =
                        EF.Functions.DateDiffDay(today, x.NextInspectionDate!.Value) == 0
                            ? "ОТК закінчується сьогодні"
                            : "Наближається закінчення ОТК"
                })
                .ToList();

            return Ok(result);
        }
    }
}