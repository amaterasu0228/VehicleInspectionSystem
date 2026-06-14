using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleService.API.Data;
using VehicleService.API.DTOs;
using VehicleService.API.Models;

namespace VehicleService.API.Controllers
{
    [ApiController]
    [Route("reminders")]
    public class RemindersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RemindersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("active")]
        public IActionResult GetActive(int days = 30)
        {
            EnsureReminders(days);

            var reminders = GetReminderQuery()
                .Where(x => !x.IsRead)
                .OrderBy(x => x.NextInspectionDate)
                .ToList();

            return Ok(reminders);
        }

        [HttpGet("read")]
        public IActionResult GetRead()
        {
            var reminders = GetReminderQuery()
                .Where(x => x.IsRead)
                .OrderByDescending(x => x.ReadAt)
                .ToList();

            return Ok(reminders);
        }

        [HttpPut("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var reminder = _context.InspectionReminders
                .FirstOrDefault(x => x.Id == id);

            if (reminder == null)
                return NotFound();

            reminder.IsRead = true;
            reminder.ReadAt = DateTime.Now;

            _context.SaveChanges();

            return Ok(reminder);
        }

        private void EnsureReminders(int days)
        {
            var today = DateTime.Today;
            var limitDate = today.AddDays(days);

            var inspections = _context.Inspections
                .Where(x => x.NextInspectionDate != null &&
                            x.NextInspectionDate >= today &&
                            x.NextInspectionDate <= limitDate)
                .ToList();

            foreach (var inspection in inspections)
            {
                var daysLeft = (inspection.NextInspectionDate!.Value.Date - today).Days;

                var reminderType = daysLeft == 0
                    ? "ОТК закінчується сьогодні"
                    : "Наближається закінчення ОТК";

                var exists = _context.InspectionReminders.Any(x =>
                    x.InspectionId == inspection.Id &&
                    x.ReminderType == reminderType);

                if (!exists)
                {
                    _context.InspectionReminders.Add(new InspectionReminder
                    {
                        InspectionId = inspection.Id,
                        ReminderType = reminderType,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }

        private IQueryable<ReminderDto> GetReminderQuery()
        {
            var today = DateTime.Today;

            return from reminder in _context.InspectionReminders
                   join inspection in _context.Inspections
                       on reminder.InspectionId equals inspection.Id
                   select new ReminderDto
                   {
                       Id = reminder.Id,
                       InspectionId = reminder.InspectionId,
                       ReminderType = reminder.ReminderType,
                       IsRead = reminder.IsRead,
                       ReadAt = reminder.ReadAt,
                       CreatedAt = reminder.CreatedAt,

                       DocumentNumber = inspection.DocumentNumber,
                       VehicleType = inspection.VehicleType,
                       Brand = inspection.Brand,
                       Model = inspection.Model,
                       Category = inspection.Category,
                       PlateNumber = inspection.PlateNumber,
                       VIN = inspection.VIN,
                       Owner = inspection.Owner,
                       OwnerPhone = inspection.OwnerPhone,
                       NextInspectionDate = inspection.NextInspectionDate,

                       DaysLeft = inspection.NextInspectionDate == null
                           ? 0
                           : EF.Functions.DateDiffDay(today, inspection.NextInspectionDate.Value)
                   };
        }
        [HttpPut("{id}/unread")]
        public IActionResult MarkAsUnread(int id)
        {
            var reminder = _context.InspectionReminders
                .FirstOrDefault(x => x.Id == id);

            if (reminder == null)
                return NotFound();

            reminder.IsRead = false;
            reminder.ReadAt = null;

            _context.SaveChanges();

            return Ok(reminder);
        }

    }
}