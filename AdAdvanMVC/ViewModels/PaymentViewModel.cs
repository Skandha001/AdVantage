// PaymentViewModel.cs (in Models/ViewModel)
using System.Collections.Generic;
using AdAdvanMVC.Models;

namespace AdAdvanMVC.Models.ViewModel
{
    public class PaymentViewModel
    {
        public List<Booking> Bookings { get; set; }
        public List<Ad> Ads { get; set; }
    }
}