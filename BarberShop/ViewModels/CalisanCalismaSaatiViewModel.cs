using BarberShop.Models;

namespace BarberShop.ViewModels
{
    public class CalisanCalismaSaatiViewModel
    {
        public DayOfWeek Gun { get; set; } // Gün (Pazartesi, Salı vb.)
        public TimeSpan BaslangicSaati { get; set; } // Başlangıç saati
        public TimeSpan BitisSaati { get; set; } // Bitiş saati
        public bool Secildi { get; set; } // Checkbox işaretlendi mi?
    }
}
