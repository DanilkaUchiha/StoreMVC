using System.Collections.Generic;

namespace StoreMVC.Models.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public List<Product> Products { get; set; }
        public List<ApplicationUser> SalesPeople { get; set; }
    }
}
