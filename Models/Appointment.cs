using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreMVC.Models
{
    public class Appointment
    {
        // Придумать ограничения для полей
        public int Id { get; set; }
        // можно email регулярку
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime AppointmentDay { get; set; }
        
        [NotMapped]//checknut'
        public DateTime AppointmentTime { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
