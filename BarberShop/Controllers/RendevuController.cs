﻿using BarberShop.Data;
using BarberShop.Models;
using BarberShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BarberShop.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RandevuController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult RandevuAl()
        {
            var model = new RandevuViewModel
            {
                RandevuTarihi = DateTime.Now
            };
            ViewBag.Calisanlar = _context.Calisanlar.ToList();
            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuAl(RandevuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                    .ThenInclude(ch => ch.Hizmet)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == model.CalisanId);

            if (calisan == null)
            {
                ModelState.AddModelError("", "Geçersiz çalışan seçimi.");
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            var hizmet = await _context.Hizmetler.FindAsync(model.HizmetId);
            if (hizmet == null)
            {
                ModelState.AddModelError("", "Geçersiz hizmet seçimi.");
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            // Uzmanlık alanı kontrolü
            bool calisanUzmanlasmismi = calisan.CalisanHizmetleri.Any(ch => ch.HizmetId == model.HizmetId);
            if (!calisanUzmanlasmismi)
            {
                ModelState.AddModelError("", "Bu çalışan seçtiğiniz hizmeti vermemektedir.");
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            var gun = model.RandevuTarihi.DayOfWeek;
            var calismaSaati = calisan.CalismaSaatleri.FirstOrDefault(cs => cs.Gun == gun);

            if (calismaSaati == null)
            {
                ModelState.AddModelError("", "Seçtiğiniz gün için çalışan çalışmıyor.");
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            var randevuSaati = model.RandevuTarihi.TimeOfDay;
            if (randevuSaati < calismaSaati.BaslangicSaati || randevuSaati > calismaSaati.BitisSaati)
            {
                ModelState.AddModelError("", "Randevu tarihi, çalışanın çalışma saatleri dışında.");
                ViewBag.Calisanlar = _context.Calisanlar.ToList();
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            var randevu = new Randevu
            {
                UserId = user.Id,
                CalisanId = model.CalisanId,
                HizmetId = model.HizmetId,
                RandevuTarihi = model.RandevuTarihi,
                Durum = RandevuDurumu.Pending
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu talebiniz alındı. Onay bekleniyor.";
            return RedirectToAction("IndexUser", "Calisan");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RandevuListele()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Hizmet)
                .ToListAsync();

            var viewModelList = new List<RandevuListeViewModel>();

            foreach (var r in randevular)
            {
                var user = await _userManager.FindByIdAsync(r.UserId);
                var userName = user?.UserName ?? r.UserId;

                viewModelList.Add(new RandevuListeViewModel
                {
                    Id = r.Id,
                    UserName = userName,
                    CalisanAdi = r.Calisan.Ad + " " + r.Calisan.Soyad,
                    HizmetAdi = r.Hizmet.Name,
                    Tarih = r.RandevuTarihi,
                    Durum = r.Durum
                });
            }

            return View(viewModelList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null) return NotFound();

            randevu.Durum = RandevuDurumu.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RandevuListele));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null) return NotFound();

            randevu.Durum = RandevuDurumu.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RandevuListele));
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Randevularim()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var randevular = await _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Hizmet)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var viewModelList = new List<RandevuListeViewModel>();
            foreach (var r in randevular)
            {
                viewModelList.Add(new RandevuListeViewModel
                {
                    Id = r.Id,
                    UserName = user.UserName,
                    CalisanAdi = r.Calisan.Ad + " " + r.Calisan.Soyad,
                    HizmetAdi = r.Hizmet.Name,
                    Tarih = r.RandevuTarihi,
                    Durum = r.Durum
                });
            }

            return View(viewModelList);
        }
    }
}
