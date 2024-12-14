using BarberShop.Data;
using BarberShop.Models;
using BarberShop.ViewModels;
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

        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar
                .Include(c => c.CalisanHizmetleri)
                    .ThenInclude(ch => ch.Hizmet)
                .Include(c => c.CalismaSaatleri) // Çalışma saatlerini dahil ediyoruz
                .ToList();

            return View(calisanlar);
        }



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
                        .Where(cs => cs.BaslangicSaati != default && cs.BitisSaati != default) // Geçerli saatleri filtrele
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

                // Çalışan bilgilerini güncelle
                calisan.Ad = model.Ad;
                calisan.Soyad = model.Soyad;
                calisan.Telefon = model.Telefon;

                // Uzmanlık alanlarını güncelle
                _context.CalisanHizmetleri.RemoveRange(calisan.CalisanHizmetleri);
                calisan.CalisanHizmetleri = model.SecilenHizmetler
                    .Select(hizmetId => new CalisanHizmet { HizmetId = hizmetId })
                    .ToList();

                // Çalışma saatlerini güncelle
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
