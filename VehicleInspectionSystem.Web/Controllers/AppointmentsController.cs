using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Models;
using VehicleInspectionSystem.Web.Services;

namespace VehicleInspectionSystem.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApiService _api;

        public AppointmentsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(
    string? status,
    DateTime? dateFrom,
    DateTime? dateTo)
        {
            var appointments = await _api.GetAppointmentsAsync(
                status,
                dateFrom,
                dateTo
            );

            ViewBag.Status = status;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AppointmentDto
            {
                AppointmentDate = DateTime.Now.AddDays(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentDto appointment)
        {
            if (!ModelState.IsValid)
                return View(appointment);

            var success = await _api.CreateAppointmentAsync(appointment);

            if (!success)
            {
                ModelState.AddModelError("", "Не вдалося створити запис на техогляд.");
                return View(appointment);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _api.UpdateAppointmentStatusAsync(id, status);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteAppointmentAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}