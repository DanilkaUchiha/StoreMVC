using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace StoreMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Sales person")]
        [MaxLength(30)]
        public string Name { get; set; }
        [NotMapped]
        public bool IsSuperAdmin { get; set; }


        public void CopyTo(ApplicationUser user)
        {
            user.Name = Name;
            user.PhoneNumber = PhoneNumber;
            user.Email = Email; // need to be confirmed again?
        }
    }
}
