using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using VehicleInspectionSystem.Web.Models;

namespace VehicleInspectionSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5093/auth/login",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Невірний email або пароль.";
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.Token))
            {
                ViewBag.Error = "Не вдалося отримати токен.";
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenResponse.Token);

            var claims = new List<Claim>();

            foreach (var claim in jwt.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }

            claims.Add(new Claim("AccessToken", tokenResponse.Token));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
            IsPersistent = true
            }
            );

            var role = claims.FirstOrDefault(c =>
    c.Type == ClaimTypes.Role ||
    c.Type == "role" ||
    c.Type.Contains("role"))?.Value;

            if (role == "Client")
                return RedirectToAction("Index", "Appointments");

            if (role == "Manager")
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Index", "Inspections");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Паролі не співпадають.";
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5093/auth/register",
                new
                {
                    email = model.Email,
                    password = model.Password
                }
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Не вдалося зареєструвати користувача.";
                return View(model);
            }

            return RedirectToAction("Login", "Auth");
        }

    }
}