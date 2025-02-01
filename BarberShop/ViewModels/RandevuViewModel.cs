using System;
using System.ComponentModel.DataAnnotations;

namespace BarberShop.ViewModels
{
    public class RandevuViewModel
    {
        [Required(ErrorMessage = "Çalışan seçimi zorunludur.")]
        public int CalisanId { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Randevu Tarihi giriniz.")]
        [DataType(DataType.DateTime)]
        public DateTime RandevuTarihi { get; set; }
    }
}
