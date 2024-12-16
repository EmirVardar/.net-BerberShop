using System.ComponentModel.DataAnnotations;

namespace BarberShop.Models
{
    // Çalışanın Çalışma Saatleri
    public class CalisanCalismaSaatleri
    {
        public int Id { get; set; }

        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        [Display(Name = "Gün")]
        public DayOfWeek Gun { get; set; } // Pazartesi, Salı vb.

        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan BaslangicSaati { get; set; }

        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan BitisSaati { get; set; }
    }
}
