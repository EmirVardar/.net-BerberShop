namespace BarberShop.Models
{
    public class CalisanCalismaSaatleri
    {
        public int Id { get; set; }
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        public DayOfWeek Gun { get; set; } // Pazartesi, Salı vb.
        public TimeSpan BaslangicSaati { get; set; } // Başlangıç saati
        public TimeSpan BitisSaati { get; set; } // Bitiş saati
    }
}
