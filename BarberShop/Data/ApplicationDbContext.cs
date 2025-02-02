﻿using BarberShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<CalisanHizmet> CalisanHizmetleri { get; set; }
        public DbSet<CalisanCalismaSaatleri> CalisanCalismaSaatleri { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İlişkiler
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

            modelBuilder.Entity<CalisanCalismaSaatleri>()
                .HasOne(cs => cs.Calisan)
                .WithMany(c => c.CalismaSaatleri)
                .HasForeignKey(cs => cs.CalisanId);
        }
    }
}
