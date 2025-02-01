using BarberShop.Models;
using System;
using System.Collections.Generic;
namespace BarberShop.ViewModels
{
    public class RandevuDetayViewModel
    {
        public string HizmetAdi { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public decimal Fiyat { get; set; }
        public int Sure { get; set; } // Süre (dakika)
    }
}
