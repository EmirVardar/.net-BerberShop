﻿using BarberShop.Data;
using BarberShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace BarberShop.Controllers
{
    [Authorize] // Genel yetkilendirme, spesifik aksiyonlarda roller belirtilir
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yalnızca Admin erişebilir
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var hizmetler = _context.Hizmetler.ToList();
            return View(hizmetler);
        }

        // Yalnızca User erişebilir
        [Authorize(Roles = "User")]
        public IActionResult IndexUser()
        {
            var hizmetler = _context.Hizmetler.ToList();
            return View(hizmetler);
        }

        // Yalnızca Admin erişebilir
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                _context.Hizmetler.Add(hizmet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Yeni hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(hizmet);
        }

        // Yalnızca Admin erişebilir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }
            return View(hizmet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hizmet hizmet)
        {
            if (id != hizmet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Hizmetler.Update(hizmet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hizmet başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(hizmet);
        }

        // Yalnızca Admin erişebilir
        // GET: Hizmet/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }
            return View(hizmet);
        }

        // POST: Hizmet/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet != null)
            {
                try
                {
                    _context.Hizmetler.Remove(hizmet);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Hizmet başarıyla silindi.";
                }
                catch (Exception ex)
                {
                    // Loglama işlemi yapılabilir
                    TempData["Error"] = "Hizmet silinirken bir hata oluştu.";
                }
            }
            else
            {
                TempData["Error"] = "Silmek istediğiniz hizmet bulunamadı.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
