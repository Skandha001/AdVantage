using System.Net.Http.Headers;
using System.Text;
using AdAdvanMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdAdvanMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string _apiBaseUrl;

        public AdminDashboardController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _apiBaseUrl = _config.GetValue<string>("WebAPIBaseUrl");
        }

        public ActionResult Index()
        {
            ViewBag.AdminEmail = HttpContext.Session.GetString("AdminEmail");
            return View();
        }

        public async Task<ActionResult> ViewAdTypes()
        {
            List<Ad> AdTypes = new List<Ad>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiBaseUrl + "/Ads"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    AdTypes = JsonConvert.DeserializeObject<List<Ad>>(apiResponse);
                }
            }

            return View(AdTypes);
        }

        // GET: AdminDashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminDashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminDashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminDashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminDashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminDashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminDashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> ManageAds()
        {
            List<Ad> ads = new List<Ad>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Ads"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ads = JsonConvert.DeserializeObject<List<Ad>>(apiResponse);
                    }
                }
            }

            return View(ads);
        }

        public async Task<IActionResult> ViewAdDetails(int id)
        {
            Ad ad = null;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Ads/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ad = JsonConvert.DeserializeObject<Ad>(apiResponse);
                    }
                }
            }
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        public async Task<IActionResult> DeleteAd(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"{_apiBaseUrl}/Ads/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ManageAds");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
        }

        public IActionResult AddAd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAd(Ad ad)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(ad), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{_apiBaseUrl}/Ads", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("ManageAds");
                        }
                        else
                        {
                            return View(ad); 
                        }
                    }
                }
            }
            return View(ad); 
        }

        public async Task<ActionResult> Approve(int id)
        {
            Booking activity = new Booking();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiBaseUrl + "/Bookings/" + id.ToString()))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    activity = JsonConvert.DeserializeObject<Booking>(apiResponse);
                }
            }
            return View(activity);
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(int id, Booking booking)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };

                string token = HttpContext.Session.GetString("JWTToken");

                using (var httpClient = new HttpClient(clientHandler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync($"{_apiBaseUrl}/Bookings/{id}", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ApproveBooking");
                    }
                    else
                    {
                        ViewBag.Message += "An error occurred while Approving the Booking." + response.ToString();
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

        public async Task<ActionResult<IEnumerable<Booking>>> ApproveBooking()
        {
            List<Booking> bookings = new List<Booking>(); // Change to List<object>

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Bookings/Pending")) // Call new endpoint
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        bookings = JsonConvert.DeserializeObject<List<Booking>>(apiResponse); // Change to List<object>
                    }
                }
            }

            return View(bookings);
        }
    }
}
