using BarberShop.Data;
using BarberShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult RandevuAl()
        {
            ViewBag.Calisanlar = _context.Calisanlar.ToList(); // Çalışanları listele
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RandevuAl(int calisanId, DateTime randevuTarihi)
        {
            var user = await _userManager.GetUserAsync(User);

            var randevu = new Randevu
            {
                KullaniciId = user.Id,
                CalisanId = calisanId,
                RandevuTarihi = randevuTarihi,
                OnaylandiMi = false
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            return RedirectToAction("RandevuListele", "Randevu");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RandevuListele()
        {
            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Kullanici)
                .ToList();

            return View(randevular);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.OnaylandiMi = true;
                _context.Randevular.Update(randevu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("RandevuListele");
        }
    }
}
