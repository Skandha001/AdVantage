// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using AdVantageWebApp.Services;
using AdVantageWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;

namespace AdVantageWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;

        public UsersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiService.PostAsync<UserModel>("users/register", model);
                    return RedirectToAction("Login");
                }
                catch
                {
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _apiService.PostAsync<dynamic>("users/login", model);
                    //HttpContext.Session.SetString("JwtToken", response.token.ToString());
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "JwtToken", response.token.ToString());
                    return RedirectToAction("Index", "Home"); // Redirect to the home page or dashboard
                }
                catch
                {
                    ModelState.AddModelError("", "Login failed. Please check your credentials.");
                }
            }
            return View(model);
        }
        //public async Task<IActionResult> Login(LoginRequest model)
        //{

        //    var response = await _apiService.PostAsJsonAsync(_WebBaseUrl + "/Users/login", model);

        //    if (!response.IsSuccessStatusCode)

        //    {

        //        return View(model);

        //    }

        //    var tokenResponse = await response.Content.ReadAsStringAsync();

        //    var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

        //    HttpContext.Session.SetString("JWToken", token.Token);

        //    HttpContext.Session.SetString("UserId", token.UserId);

        //    HttpContext.Session.SetString("Role", token.Role);

        //    return RedirectToAction("Index", "Home");

        //}


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }
    }
}