using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreMVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public bool Available { get; set; }
        public string Image { get; set; }
        [Display(Name = "Shade color")]
        public string ShadeColor { get; set; }

        [Display(Name = "Product type")]
        public int ProductTypeId { get; set; }
        [ForeignKey(nameof(ProductTypeId))]
        [Display(Name = "Product type")]
        public virtual ProductType ProductType { get; set; }
        
        [Display(Name = "Special tag")]
        public int SpecialTagId { get; set; }
        [ForeignKey(nameof(SpecialTagId))]
        [Display(Name = "Special tag")]
        public virtual SpecialTag SpecialTag { get; set; }

        public void CopyTo(Product product)
        {
            product.Name = Name;
            product.Image = Image;
            product.Price = Price;
            product.SpecialTagId = SpecialTagId;
            product.ProductTypeId = ProductTypeId;
            product.ShadeColor = ShadeColor;
            product.Available = Available;
        }
    }
}
