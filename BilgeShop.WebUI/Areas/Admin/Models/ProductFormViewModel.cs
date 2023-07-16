using System.ComponentModel.DataAnnotations;

namespace BilgeShop.WebUI.Areas.Admin.Models
{
    public class ProductFormViewModel
    {
        
        public int Id { get; set; }

        [Display(Name = "İsim")]
        [Required(ErrorMessage = "Ürün ismi zorunludur")]
        public string Name { get; set; }

        [Display(Name = "Açklama")]
        public string Description { get; set; }

        [Display(Name = "Ürün Fiyatı")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Stok")]
        public int UnitStock { get; set; }

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori zorunludur")]
        public int CategoryId { get; set; }

        [Display(Name = "Ürün Görseli")]
        public IFormFile? File { get; set; }

    }
}
