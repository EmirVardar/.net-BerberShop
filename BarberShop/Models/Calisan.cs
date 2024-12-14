namespace BarberShop.Models
{
    public class Calisan
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Telefon { get; set; }

        // Çalışanın uzmanlık alanları (Hizmetler)
        public List<CalisanHizmet> CalisanHizmetleri { get; set; } = new List<CalisanHizmet>();
        public List<CalisanCalismaSaatleri> CalismaSaatleri { get; set; } = new List<CalisanCalismaSaatleri>();
    }
}
