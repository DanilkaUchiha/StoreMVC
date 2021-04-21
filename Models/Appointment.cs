using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreMVC.Models
{
    public class Appointment
    {
        // Придумать ограничения для полей
        public int Id { get; set; }
        // можно email регулярку
        [DisplayName("Email")]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [DisplayName("Phone number")]
        public string CustomerPhoneNumber { get; set; }
        [DisplayName("Full name")]
        [Required, MinLength(2), MaxLength(30)]
        public string CustomerName { get; set; }
        [DisplayName("Appointment date")]
        public DateTime AppointmentDay { get; set; }
        
        [Display(Name="Sales person")]
        public string SalesPersonId { get; set; }
        [ForeignKey(nameof(SalesPersonId))]
        public ApplicationUser SalesPerson { get; set; }


        [NotMapped]//checknut'
        public DateTime AppointmentTime { get; set; }
        [DisplayName("Confirmed")]
        public bool IsConfirmed { get; set; }
        public void CopyTo(Appointment appointment)
        {
            appointment.AppointmentDay = AppointmentDay
                .AddHours(AppointmentTime.Hour)
                .AddMinutes(AppointmentTime.Minute);

            appointment.CustomerName = CustomerName;
            appointment.CustomerEmail = CustomerEmail;
            appointment.CustomerPhoneNumber = CustomerPhoneNumber;
            appointment.IsConfirmed = IsConfirmed;
        }

    }
}
