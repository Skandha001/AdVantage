using System.Diagnostics;
using AdVantageWebApp.Models;
using AdVantageWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AdVantageWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;

        public async Task<IActionResult> BookingHistory()
        {
            try
            {
                // Get the JWT token and user ID from the session
                var token = HttpContext.Session.GetString("JwtToken");
                var userId = HttpContext.Session.GetInt32("UserId");

                if (string.IsNullOrEmpty(token) || !userId.HasValue)
                {
                    return RedirectToAction("Login", "Users"); // Redirect if not logged in
                }

                _apiService.SetAuthorizationHeader(token);

                // Fetch all bookings from the API
                var allBookings = await _apiService.GetAsync<List<BookingModel>>("bookings");

                // Filter bookings for the current user
                var userBookings = allBookings.Where(b => b.UserId == userId).ToList();

                _apiService.ClearAuthorizationHeader();

                return View(userBookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve booking history");
                return View(new List<BookingModel>()); // Return an empty list on error
            }
        }

        [HttpGet]
        public async Task<IActionResult> BookAd(int id)
        {
            var ad = await _apiService.GetAsync<AdModel>($"ads/{id}");
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        [HttpPost]
        public async Task<IActionResult> BookAd(BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the JWT token from the session
                    var token = HttpContext.Session.GetString("JwtToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return RedirectToAction("Login", "Users"); // Redirect to login if no token
                    }

                    _apiService.SetAuthorizationHeader(token); // Set the authorization header

                    await _apiService.PostAsync<BookingModel>("bookings", booking);

                    _apiService.ClearAuthorizationHeader(); // Clear the authorization header

                    return RedirectToAction("Index"); // Redirect to the home page
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Booking failed. Please try again.");
                    _logger.LogError(ex, "Booking failed");
                }
            }

            var ad = await _apiService.GetAsync<AdModel>($"ads/{booking.AdId}");
            return View(ad);
        }


        [HttpGet]
        public async Task<IActionResult> Feedback(int adId)
        {
            var ad = await _apiService.GetAsync<AdModel>($"ads/{adId}");
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(FeedbackModel feedback)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = HttpContext.Session.GetString("JwtToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return RedirectToAction("Login", "Users");
                    }

                    _apiService.SetAuthorizationHeader(token);

                    feedback.UserId = HttpContext.Session.GetInt32("UserId").Value;
                    feedback.FeedbackDate = DateTime.UtcNow;

                    await _apiService.PostAsync<FeedbackModel>("feedbacks", feedback);

                    _apiService.ClearAuthorizationHeader();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Feedback submission failed. Please try again.");
                    _logger.LogError(ex, "Feedback submission failed");
                }
            }

            var ad = await _apiService.GetAsync<AdModel>($"ads/{feedback.AdId}");
            return View(ad);
        }


        public HomeController(ILogger<HomeController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

        }

        public async Task<IActionResult> Index()
        {
            //return View();
            try
            {
                var ads = await _apiService.GetAsync<List<AdModel>>("ads/available");
                return View(ads);
            }
            catch
            {
                return View(new List<AdModel>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
