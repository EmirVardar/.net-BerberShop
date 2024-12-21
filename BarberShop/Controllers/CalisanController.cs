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



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null) return NotFound();

            // Tüm günler ve çalışanın çalışma saatlerini birleştir
            var tumGunler = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
            var calismaSaatleri = tumGunler.Select(gun =>
            {
                var mevcutGun = calisan.CalismaSaatleri.FirstOrDefault(cs => cs.Gun == gun);
                return new CalisanCalismaSaatiViewModel
                {
                    Gun = gun,
                    BaslangicSaati = mevcutGun?.BaslangicSaati ?? TimeSpan.Zero,
                    BitisSaati = mevcutGun?.BitisSaati ?? TimeSpan.Zero,
                    Secildi = mevcutGun != null
                };
            }).ToList();

            var viewModel = new CalisanViewModel
            {
                Id = calisan.Id,
                Ad = calisan.Ad,
                Soyad = calisan.Soyad,
                Telefon = calisan.Telefon,
                SecilenHizmetler = calisan.CalisanHizmetleri.Select(ch => ch.HizmetId).ToList(),
                CalismaSaatleri = calismaSaatleri
            };

            ViewBag.Hizmetler = _context.Hizmetler.ToList(); // Uzmanlık alanları için doldur
            return View(viewModel);
        }




        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CalisanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Hizmetler = _context.Hizmetler.ToList(); // Uzmanlık alanları için tekrar doldur
                return View(model);
            }

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
            calisan.CalismaSaatleri = model.CalismaSaatleri
                .Where(cs => cs.Secildi) // Sadece seçili olan günleri ekle
                .Select(cs => new CalisanCalismaSaatleri
                {
                    Gun = cs.Gun,
                    BaslangicSaati = cs.BaslangicSaati,
                    BitisSaati = cs.BitisSaati
                })
                .ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CalisanDetay(int id)
        {
            // Çalışanı getir
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null) return NotFound();

            // Onaylanmış randevuları getir
            var randevular = await _context.Randevular
                .Include(r => r.Hizmet)
                .Where(r => r.CalisanId == id && r.Durum == RandevuDurumu.Approved)
                .ToListAsync();

            // Toplam kazanç
            decimal toplamKazanc = randevular.Sum(r => r.Hizmet.Price);

            // Toplam çalışma süresi
            int toplamCalismaDakika = randevular.Sum(r => r.Hizmet.Duration);

            // ViewModel'e verileri aktar
            var model = new CalisanDetayViewModel
            {
                CalisanId = calisan.Id,
                Ad = calisan.Ad,
                Soyad = calisan.Soyad,
                ToplamKazanc = toplamKazanc,
                ToplamCalismaDakika = toplamCalismaDakika,
                Randevular = randevular.Select(r => new RandevuDetayViewModel
                {
                    HizmetAdi = r.Hizmet.Name,
                    RandevuTarihi = r.RandevuTarihi,
                    Fiyat = r.Hizmet.Price,
                    Sure = r.Hizmet.Duration
                }).ToList()
            };

            return View(model);
        }


    }
}
