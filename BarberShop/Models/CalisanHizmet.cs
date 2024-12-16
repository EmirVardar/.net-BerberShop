namespace BarberShop.Models
{
    // Çalışan ile Hizmet ilişkisi
    public class CalisanHizmet
    {
        public int Id { get; set; }
        public int CalisanId { get; set; }
        public Calisan Calisan { get; set; }

        public int HizmetId { get; set; }
        public Hizmet Hizmet { get; set; }
    }
}
