using System.Collections.Generic;

namespace StoreMVC.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();
    }
}
