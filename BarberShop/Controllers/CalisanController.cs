using BarberShop.Data;
using BarberShop.Models;
using BarberShop.ViewModels; // ViewModel'ler için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberShop.Controllers
{
    
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalisanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm çalışanları listele
        // Admin rolü yetkisi gerekiyor
        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanHizmetleri) // Uzmanlık alanlarını dahil et
                    .ThenInclude(ch => ch.Hizmet) // Hizmet detaylarını dahil et
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .ToList();

            return View(calisanlar);
        }

        [Authorize(Roles = "User")]
        public IActionResult IndexUser()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanHizmetleri) // Uzmanlık alanlarını dahil et
                    .ThenInclude(ch => ch.Hizmet) // Hizmet detaylarını dahil et
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil et
                .ToList();

            return View(calisanlar);
        }


        // Yeni çalışan ekleme sayfası
        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        public IActionResult Create()
        {
            ViewBag.Hizmetler = _context.Hizmetler.ToList(); // Hizmetleri dropdown için al
            return View(new CalisanViewModel());
        }
        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalisanViewModel model)
        {
            // Sadece seçilen günleri filtrele
            var seciliGunler = model.CalismaSaatleri
                .Where(cs => cs.Secildi && cs.BaslangicSaati != default && cs.BitisSaati != default) // Saatler dolu mu?
                .ToList();

            // ModelState'i temizle (gereksiz validasyon hatalarını önlemek için)
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var calisan = new Calisan
                {
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Telefon = model.Telefon,
                    CalisanHizmetleri = model.SecilenHizmetler.Select(hizmetId => new CalisanHizmet
                    {
                        HizmetId = hizmetId
                    }).ToList(),
                    CalismaSaatleri = seciliGunler.Select(cs => new CalisanCalismaSaatleri
                    {
                        Gun = cs.Gun,
                        BaslangicSaati = cs.BaslangicSaati,
                        BitisSaati = cs.BitisSaati
                    }).ToList()
                };

                // Veritabanına kaydet
                _context.Calisanlar.Add(calisan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            return View(model);
        }



        // Çalışan düzenleme
        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        public async Task<IActionResult> Edit(int id)
        {
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null) return NotFound();

            var viewModel = new CalisanViewModel
            {
                Id = calisan.Id,
                Ad = calisan.Ad,
                Soyad = calisan.Soyad,
                Telefon = calisan.Telefon,
                SecilenHizmetler = calisan.CalisanHizmetleri.Select(ch => ch.HizmetId).ToList(),
                CalismaSaatleri = calisan.CalismaSaatleri.Select(cs => new CalisanCalismaSaatiViewModel
                {
                    Gun = cs.Gun,
                    BaslangicSaati = cs.BaslangicSaati,
                    BitisSaati = cs.BitisSaati
                }).ToList()
            };

            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CalisanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var calisan = await _context.Calisanlar
                    .Include(c => c.CalisanHizmetleri)
                    .Include(c => c.CalismaSaatleri)
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (calisan == null) return NotFound();

                calisan.Ad = model.Ad;
                calisan.Soyad = model.Soyad;
                calisan.Telefon = model.Telefon;

                // Uzmanlık alanlarını güncelle
                _context.CalisanHizmetleri.RemoveRange(calisan.CalisanHizmetleri);
                calisan.CalisanHizmetleri = model.SecilenHizmetler.Select(hizmetId => new CalisanHizmet
                {
                    HizmetId = hizmetId
                }).ToList();

                // Çalışma saatlerini güncelle
                _context.CalisanCalismaSaatleri.RemoveRange(calisan.CalismaSaatleri);
                calisan.CalismaSaatleri = model.CalismaSaatleri.Select(cs => new CalisanCalismaSaatleri
                {
                    Gun = cs.Gun,
                    BaslangicSaati = cs.BaslangicSaati,
                    BitisSaati = cs.BitisSaati
                }).ToList();

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var calisan = await _context.Calisanlar
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null) return NotFound();

            return View(calisan);
        }

        [Authorize(Roles = "Admin")] // Admin rolü yetkisi gerekiyor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null) return NotFound();

            // İlişkili verileri sil
            _context.CalisanHizmetleri.RemoveRange(calisan.CalisanHizmetleri);
            _context.CalisanCalismaSaatleri.RemoveRange(calisan.CalismaSaatleri);

            // Çalışanı sil
            _context.Calisanlar.Remove(calisan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
