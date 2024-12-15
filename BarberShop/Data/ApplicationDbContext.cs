namespace BarberShop.Data
{
    using BarberShop.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<CalisanHizmet> CalisanHizmetleri { get; set; }
        public DbSet<CalisanCalismaSaatleri> CalisanCalismaSaatleri { get; set; }



        // Veritabanı tablolarınızı temsil eden DbSet'ler:
        // public DbSet<Calisan> Calisanlar { get; set; }
        // public DbSet<Islem> Islemler { get; set; }
        // ...
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Çalışan - Hizmet ilişkisi
            modelBuilder.Entity<CalisanHizmet>()
                .HasKey(ch => new { ch.CalisanId, ch.HizmetId });

            modelBuilder.Entity<CalisanHizmet>()
                .HasOne(ch => ch.Calisan)
                .WithMany(c => c.CalisanHizmetleri)
                .HasForeignKey(ch => ch.CalisanId);

            modelBuilder.Entity<CalisanHizmet>()
                .HasOne(ch => ch.Hizmet)
                .WithMany()
                .HasForeignKey(ch => ch.HizmetId);

            // Çalışan - Çalışma Saatleri ilişkisi
            modelBuilder.Entity<CalisanCalismaSaatleri>()
    .HasOne(cs => cs.Calisan)
    .WithMany(c => c.CalismaSaatleri)
    .HasForeignKey(cs => cs.CalisanId);


        }
    }

}
