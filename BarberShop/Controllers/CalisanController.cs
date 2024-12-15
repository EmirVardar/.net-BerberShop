using BarberShop.Data;
using BarberShop.Models;
using BarberShop.ViewModels;
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

        // Yalnızca Admin erişebilir
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                    .ThenInclude(ch => ch.Hizmet)
                .Include(c => c.CalismaSaatleri)
                .ToList();

            return View(calisanlar);
        }

        // Yalnızca User erişebilir
        [Authorize(Roles = "User")]
        public IActionResult IndexCalisanlar()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                    .ThenInclude(ch => ch.Hizmet)
                .Include(c => c.CalismaSaatleri)
                .ToList();

            return View(calisanlar);
        }

        // Admin Erişimli
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new CalisanViewModel
            {
                TumHizmetler = _context.Hizmetler.ToList(),
                CalismaSaatleri = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .Select(gun => new CalisanCalismaSaatiViewModel { Gun = gun })
                    .ToList()
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalisanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var calisan = new Calisan
                {
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Telefon = model.Telefon,
                    CalismaSaatleri = model.CalismaSaatleri
                        .Where(cs => cs.BaslangicSaati != default && cs.BitisSaati != default)
                        .Select(cs => new CalisanCalismaSaatleri
                        {
                            Gun = cs.Gun,
                            BaslangicSaati = cs.BaslangicSaati,
                            BitisSaati = cs.BitisSaati
                        }).ToList()
                };

                _context.Calisanlar.Add(calisan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.TumHizmetler = _context.Hizmetler.ToList();
            return View(model);
        }

        // Admin Erişimli
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null)
            {
                return NotFound();
            }

            var viewModel = new CalisanViewModel
            {
                Id = calisan.Id,
                Ad = calisan.Ad,
                Soyad = calisan.Soyad,
                Telefon = calisan.Telefon,
                SecilenHizmetler = calisan.CalisanHizmetleri.Select(ch => ch.HizmetId).ToList(),
                TumHizmetler = _context.Hizmetler.ToList(),
                CalismaSaatleri = calisan.CalismaSaatleri.Select(cs => new CalisanCalismaSaatiViewModel
                {
                    Gun = cs.Gun,
                    BaslangicSaati = cs.BaslangicSaati,
                    BitisSaati = cs.BitisSaati
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
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

                if (calisan == null)
                {
                    return NotFound();
                }

                calisan.Ad = model.Ad;
                calisan.Soyad = model.Soyad;
                calisan.Telefon = model.Telefon;

                _context.CalisanHizmetleri.RemoveRange(calisan.CalisanHizmetleri);
                calisan.CalisanHizmetleri = model.SecilenHizmetler
                    .Select(hizmetId => new CalisanHizmet { HizmetId = hizmetId })
                    .ToList();

                _context.CalisanCalismaSaatleri.RemoveRange(calisan.CalismaSaatleri);
                calisan.CalismaSaatleri = model.CalismaSaatleri
                    .Where(cs => cs.BaslangicSaati != default && cs.BitisSaati != default)
                    .Select(cs => new CalisanCalismaSaatleri
                    {
                        Gun = cs.Gun,
                        BaslangicSaati = cs.BaslangicSaati,
                        BitisSaati = cs.BitisSaati
                    })
                    .ToList();

                _context.Calisanlar.Update(calisan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.TumHizmetler = _context.Hizmetler.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var calisan = await _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                .Include(c => c.CalismaSaatleri)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calisan == null)
            {
                return NotFound();
            }

            _context.CalisanHizmetleri.RemoveRange(calisan.CalisanHizmetleri);
            _context.CalisanCalismaSaatleri.RemoveRange(calisan.CalismaSaatleri);
            _context.Calisanlar.Remove(calisan);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
