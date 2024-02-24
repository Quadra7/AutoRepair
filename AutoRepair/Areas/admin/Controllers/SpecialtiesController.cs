using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Data;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class SpecialtiesController : Controller
    {
        private readonly ApplicationContext _context;

        public SpecialtiesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index() => _context.Specialty != null ?
            View(await _context.Specialty.ToListAsync()) :
            Problem("Entity set 'ApplicationContext.Specialties' is null.");

        // GET: Specialties/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Specialty == null)
                return NotFound();

            var specialtyModel = await _context.Specialty.FindAsync(ID);

            return specialtyModel != null ? View(specialtyModel) : NotFound();
        }

        // GET: Specialties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,Name,Description")] SpecialtyModel specialtyModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialtyModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(specialtyModel);
        }

        // GET: Specialties/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Specialty == null)
                return NotFound();

            var specialtyModel = await _context.Specialty.FindAsync(ID);

            return specialtyModel != null ? View(specialtyModel) : NotFound();
        }

        // POST: Specialties/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,Name,Description")] SpecialtyModel specialtyModel)
        {
            if (ID != specialtyModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialtyModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index");
            }

            return View(specialtyModel);
        }

        // GET: Specialties/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Specialty == null)
                return NotFound();

            var specialtyModel = await _context.Specialty.FindAsync(ID);

            return specialtyModel != null ? View(specialtyModel) : NotFound();
        }

        // POST: Specialties/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            if (_context.Specialty == null)
                return Problem("Entity set 'ApplicationContext.Specilaties' is null.");

            var specialtyModel = await _context.Specialty.FindAsync(ID);

            if (specialtyModel != null)
                _context.Specialty.Remove(specialtyModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool SpecialtyModelExists(Guid ID)
            => (_context.Specialty?.Any(s => s.ID == ID)).GetValueOrDefault();
    }
}