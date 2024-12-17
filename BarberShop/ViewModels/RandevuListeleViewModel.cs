using BarberShop.Models;
using System;

namespace BarberShop.ViewModels
{
    public class RandevuListeViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string CalisanAdi { get; set; }
        public string HizmetAdi { get; set; }
        public DateTime Tarih { get; set; }
        public RandevuDurumu Durum { get; set; }
    }
}
