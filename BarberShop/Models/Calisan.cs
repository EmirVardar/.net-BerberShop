using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    public class Calisan
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ad")]
        public string Ad { get; set; }

        [Required]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; }

        [Required]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; }

        // Uzmanlık alanı: Çalışanın hizmetler tablosuyla ilişkili olduğu alanlar
        public List<CalisanHizmet> CalisanHizmetleri { get; set; } = new List<CalisanHizmet>();

        // Çalışma Saatleri
        public List<CalisanCalismaSaatleri> CalismaSaatleri { get; set; } = new List<CalisanCalismaSaatleri>();
    }

}
