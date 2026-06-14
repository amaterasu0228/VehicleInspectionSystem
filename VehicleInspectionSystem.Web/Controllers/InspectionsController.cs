using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Models;
using VehicleInspectionSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace VehicleInspectionSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Operator,Expert,Manager")]
    public class InspectionsController : Controller
    {
        private readonly ApiService _api;

        public InspectionsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(
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
            var data = await _api.FilterInspectionsAsync(
                plate,
                vin,
                owner,
                brand,
                model,
                dateFrom,
                dateTo,
                sortBy,
                sortDirection
            );

            ViewBag.Plate = plate;
            ViewBag.Vin = vin;
            ViewBag.Owner = owner;
            ViewBag.Brand = brand;
            ViewBag.Model = model;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;

            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inspection = await _api.GetInspectionByIdAsync(id);

            if (inspection == null)
                return NotFound();

            return View(inspection);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inspection = await _api.GetInspectionByIdAsync(id);

            if (inspection == null)
                return NotFound();

            return View(inspection);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, InspectionDto inspection)
        {
            if (!ModelState.IsValid)
                return View(inspection);

            var success = await _api.UpdateInspectionAsync(id, inspection);

            if (!success)
            {
                ModelState.AddModelError("", "Не вдалося оновити запис.");
                return View(inspection);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteInspectionAsync(id);

            if (!success)
                return NotFound();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new InspectionDto
            {
                SignDate = DateTime.Today,
                SignTime = DateTime.Now.TimeOfDay,
                NextInspectionDate = DateTime.Today.AddYears(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(InspectionDto inspection)
        {
            if (!ModelState.IsValid)
                return View(inspection);

            var success = await _api.CreateInspectionAsync(inspection);

            if (!success)
            {
                ModelState.AddModelError("", "Не вдалося створити запис.");
                return View(inspection);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}