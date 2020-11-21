using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace StoreMVC.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Имя обязательно!")]
        [MinLength(2, ErrorMessage = "Минимальная длина должна быть больше 1"), MaxLength(30, ErrorMessage = "Максимальная длина должна быть меньше 31")]
        public string Name { get; set; }
    }
}
