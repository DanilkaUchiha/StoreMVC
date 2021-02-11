using System.ComponentModel.DataAnnotations;

namespace StoreMVC.Models
{
    public class NamedDbItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        [MinLength(2, ErrorMessage = "Min length must be greater than 1"), MaxLength(30, ErrorMessage = "Maximum length must be less than 31")]
        public string Name { get; set; }
    }
}
