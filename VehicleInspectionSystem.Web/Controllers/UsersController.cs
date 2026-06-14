using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Services;

namespace VehicleInspectionSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApiService _api;

        public UsersController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _api.GetUsersAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            await _api.UpdateUserRoleAsync(id, role);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteUserAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}