using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using AdAdvanMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdAdvanMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string _apiBaseUrl;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _apiBaseUrl = _config.GetValue<string>("WebAPIBaseUrl");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(User user)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();

            clientHandler.ServerCertificateCustomValidationCallback
                = (sender, cert, chain, SslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(_apiBaseUrl + "/Token", stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync();
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Message = "Incorrect Email and Password";
                        return View();
                    }
                    HttpContext.Session.SetString("JWTToken", token);
                    HttpContext.Session.SetString("AdminEmail", user.Email);
                }
                return Redirect("~/AdminDashboard/Index");
            }
        }

        [HttpGet]
        public IActionResult ClientLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientLogin(User user)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();

            clientHandler.ServerCertificateCustomValidationCallback
                = (sender, cert, chain, SslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(_apiBaseUrl + "/Token", stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync();
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Message = "Incorrect Email and Password";
                        return View();
                    }
                    HttpContext.Session.SetString("JWTToken", token);
                    HttpContext.Session.SetString("ClientEmail", user.Email);
                }
                return Redirect("~/ClientDashboard/Index");
            }
        }

        [HttpGet]
        public IActionResult ClientRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientRegistration(IFormCollection collection)
        {
            User user = new()
            {
                UserID = 0,
                Username = collection["Username"], 
                PhoneNumber = collection["PhoneNumber"],
                Email = collection["Email"],
                Password = collection["Password"],
            };
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };

                string token = HttpContext.Session.GetString("JWTToken"); // Replace with your JWT token

                using (var httpClient = new HttpClient(clientHandler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    ViewBag.Message = stringContent.ToString();

                    var response = await httpClient.PostAsync(_apiBaseUrl + "/users", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ClientLogin","Home");
                    }
                    else
                    {
                        ViewBag.Message = "An error occurred while creating the activity.";
                        return View();
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Message += e.ToString();
                return View();
            }
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
