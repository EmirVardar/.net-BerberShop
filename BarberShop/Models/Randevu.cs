using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Models
{
    public enum RandevuDurumu
    {
        Pending,
        Approved,
        Rejected
    }

    public class Randevu
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int CalisanId { get; set; }

        [ForeignKey("CalisanId")]
        public Calisan Calisan { get; set; }

        [Required]
        public int HizmetId { get; set; }

        [ForeignKey("HizmetId")]
        public Hizmet Hizmet { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RandevuTarihi { get; set; }

        public RandevuDurumu Durum { get; set; } = RandevuDurumu.Pending;
    }
}
