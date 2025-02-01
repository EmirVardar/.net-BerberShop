using BarberShop.Models;
using System;
using System.Collections.Generic;
namespace BarberShop.ViewModels
{
    public class CalisanDetayViewModel
    {
        public int CalisanId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public decimal ToplamKazanc { get; set; }
        public int ToplamCalismaDakika { get; set; }
        public List<RandevuDetayViewModel> Randevular { get; set; }
    }
}
