namespace BarberShop.Models
{
    public class Hizmet
    {
        public int Id { get; set; }
        public string Name { get; set; } // Hizmet adı
        public string Description { get; set; } // Hizmet açıklaması
        public decimal Price { get; set; } // Hizmet ücreti
        public int Duration { get; set; } // Hizmet süresi (dakika)
        public List<CalisanHizmet> CalisanHizmetleri { get; set; } = new List<CalisanHizmet>();
    }
}
