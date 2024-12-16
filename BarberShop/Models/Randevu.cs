using Microsoft.AspNetCore.Identity;
using System;



namespace BarberShop.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        public string KullaniciId { get; set; } // Kullanıcı ID'si
        public IdentityUser Kullanici { get; set; } // Navigation Property

        public int CalisanId { get; set; } // Çalışan ID'si
        public Calisan Calisan { get; set; } // Navigation Property

        public DateTime RandevuTarihi { get; set; } // Randevu tarihi ve saati
        public bool OnaylandiMi { get; set; } = false; // Randevu durumu (Admin onayı)
    }
}
