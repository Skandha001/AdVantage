using System.Net.Http.Headers;
using System.Text;
using AdAdvanMVC.Models;
using AdAdvanMVC.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace AdAdvanMVC.Controllers
{
    public class ClientDashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string _apiBaseUrl;

        public ClientDashboardController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _apiBaseUrl = _config.GetValue<string>("WebAPIBaseUrl");

        }
        public async Task<IActionResult> Index()
        {
            ViewBag.WelcomeMsg = "Welcome " + HttpContext.Session.GetString("ClientEmail");
            return View(await GetAdTypes());
        }

        public async Task<List<Ad>> GetAdTypes()
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

            return AdTypes;
        }


        // GET: ClientDashboardController/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        public async Task<ActionResult> Book(int id)
        {
            Ad adv = new Ad();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiBaseUrl + "/Ads/" + id.ToString()))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    adv = JsonConvert.DeserializeObject<Ad>(apiResponse);
                }
            }
            ViewBag.Ad = adv;

            return View();
        }

        // POST: ClientDashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Book(int id, IFormCollection collection)
        {
            int BookingUserID;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiBaseUrl + "/Users"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                    User user = users.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("ClientEmail"));
                    BookingUserID = user.UserID;
                }
            }

            Booking booking = new()
            {
                BookingID = 0,
                UserID = BookingUserID,
                AdID = id,
                BookingDate = Convert.ToDateTime(collection["BookingDate"]),
                StartDate = Convert.ToDateTime(collection["StartDate"]),
                EndDate = Convert.ToDateTime(collection["EndDate"]),
                IsApproved = Convert.ToBoolean(collection["IsApproved"]),
            };

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };

                string token = HttpContext.Session.GetString("JWTToken"); // Replace with your JWT token

                using (var httpClient = new HttpClient(clientHandler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
                    ViewBag.Message = stringContent.ToString();

                    var response = await httpClient.PostAsync(_apiBaseUrl + "/Bookings", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
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

        // GET: ClientDashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClientDashboardController/Delete/5
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
        public async Task<IActionResult> ViewMyAds()
        {
            int clientId;

            // Retrieve the client's UserID from the session
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiBaseUrl + "/Users"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                    User user = users.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("ClientEmail"));
                    clientId = user.UserID;
                }
            }

            // Retrieve ALL bookings from the API
            List<Booking> allBookings = new List<Booking>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Bookings")) // Assuming you have an endpoint that gets all bookings
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        allBookings = JsonConvert.DeserializeObject<List<Booking>>(apiResponse);
                    }
                }
            }

            // Filter bookings based on the client's ID
            List<Booking> clientBookings = allBookings.Where(booking => booking.UserID == clientId).ToList();

            // Retrieve the ads associated with the bookings
            List<Ad> clientAds = new List<Ad>();
            using (var httpClient = new HttpClient())
            {
                foreach (var booking in clientBookings)
                {
                    using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Ads/{booking.AdID}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            Ad ad = JsonConvert.DeserializeObject<Ad>(apiResponse);
                            clientAds.Add(ad);
                        }
                    }
                }
            }

            return View(clientAds);
        }




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MakePayment(IFormCollection collection)
{
    // Retrieve booking ID from form collection
    int bookingId = Convert.ToInt32(collection["BookingID"]);
    string paymentMethod = collection["PaymentMethod"];

    // Retrieve booking from API
    Booking booking = null;
    using (var httpClient = new HttpClient())
    {
        using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Bookings/{bookingId}"))
        {
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                booking = JsonConvert.DeserializeObject<Booking>(apiResponse);
            }
        }
    }

    if (booking == null)
    {
        return NotFound();
    }

    // Retrieve Ad info.
    Ad ad = null;
    using (var httpClient = new HttpClient())
    {
        using (var response = await httpClient.GetAsync($"{_apiBaseUrl}/Ads/{booking.AdID}"))
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

    // Create payment record
    Payment payment = new Payment
    {
        BookingId = bookingId,
        PaymentDate = DateTime.Now,
        Amount = ad.PricePerDayOrOneIssue, // Get price from Ad
        PaymentMethod = paymentMethod,
        IsSuccessful = true // Assuming payment is successful
    };

    // Post payment to API
    using (var httpClient = new HttpClient())
    {
        StringContent stringContent = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(_apiBaseUrl + "/Payments", stringContent);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Message = "Payment failed.";
            return View();
        }
    }

    // Update booking status to paid (or delete booking) to remove from "ViewMyAds"
    using (var httpClient = new HttpClient())
    {
        booking.IsApproved = false; // Or any other status you choose to indicate paid.

        StringContent stringContent = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"{_apiBaseUrl}/Bookings/{bookingId}", stringContent); // Or use DELETE if you want to delete the booking.

        if (!response.IsSuccessStatusCode)
        {
            // Handle the case where the booking update fails
            ViewBag.Message = "Payment successful, but booking update failed.";
            return View(); // Or redirect with an error message
        }
    }

    return RedirectToAction("ViewMyAds");
}

       
    }
}
