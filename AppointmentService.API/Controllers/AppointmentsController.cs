using AppointmentService.API.Data;
using AppointmentService.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers
{
    [ApiController]
    [Route("appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(
    string? status,
    DateTime? dateFrom,
    DateTime? dateTo)
        {
            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (dateFrom.HasValue)
                query = query.Where(x => x.AppointmentDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(x => x.AppointmentDate <= dateTo.Value);

            var appointments = query
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var appointment = _context.Appointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Appointment appointment)
        {
            appointment.Status = "Новий";
            appointment.CreatedAt = DateTime.Now;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok(appointment);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Appointment updated)
        {
            var appointment = _context.Appointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.ClientName = updated.ClientName;
            appointment.Phone = updated.Phone;
            appointment.Email = updated.Email;

            appointment.VehicleBrand = updated.VehicleBrand;
            appointment.VehicleModel = updated.VehicleModel;
            appointment.PlateNumber = updated.PlateNumber;

            appointment.AppointmentDate = updated.AppointmentDate;
            appointment.Status = updated.Status;

            _context.SaveChanges();

            return Ok(appointment);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] string status)
        {
            var appointment = _context.Appointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = status;

            _context.SaveChanges();

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments
                .FirstOrDefault(x => x.Id == id);

            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            var total = _context.Appointments.Count();

            var newCount = _context.Appointments
                .Count(x => x.Status == "Новий");

            var confirmedCount = _context.Appointments
                .Count(x => x.Status == "Підтверджено");

            var completedCount = _context.Appointments
                .Count(x => x.Status == "Виконано");

            var rejectedCount = _context.Appointments
                .Count(x => x.Status == "Відхилено");

            return Ok(new
            {
                total,
                newCount,
                confirmedCount,
                completedCount,
                rejectedCount
            });
        }

    }

}