using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Data;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationContext _context;

        public ServicesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = _context.Service.Include(s => s.Specialty);

            return services != null ?
                View(await services.ToListAsync()) :
                Problem("Entity set 'ApplicationContext.Services' is null.");
        }

        // GET: Services/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Service == null)
                return NotFound();

            var serviceModel = await _context.Service
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(s => s.ID == ID);

            return serviceModel != null ? View(serviceModel) : NotFound();
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            ViewBag.Specialties = new SelectList(_context.Specialty, "ID", "Name");

            return View();
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,Name,Description,Price,SpecialtyID")] ServiceModel serviceModel)
        {
            if (ModelState.IsValid)
            {
                _context.Service.Add(serviceModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewData["SpecialtyID"] = new SelectList(_context.Specialty, "ID", "Name", serviceModel.SpecialtyID);

            return View(serviceModel);
        }

        // GET: Services/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Service == null)
                return NotFound();

            var serviceModel = await _context.Service.FindAsync(ID);

            if (serviceModel == null)
                return NotFound();

            ViewData["SpecialtyID"] = new SelectList(_context.Specialty, "ID", "Name", serviceModel.SpecialtyID);

            return View(serviceModel);
        }

        // POST: Services/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,Name,Description,Price,SpecialtyID")] ServiceModel serviceModel)
        {
            if (ID != serviceModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index");
            }

            ViewData["SpecialtyID"] = new SelectList(_context.Specialty, "ID", "Name", serviceModel.SpecialtyID);

            return View(serviceModel);
        }

        // GET: Services/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Service == null)
                return NotFound();

            var service = await _context.Service
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(s => s.ID == ID);

            return service != null ? View(service) : NotFound();
        }

        // POST: Services/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            if (_context.Service == null)
                return Problem("Entity set 'ApplicationContext.Services' is null.");

            var serviceModel = await _context.Service.FindAsync(ID);

            if (serviceModel != null)
                _context.Service.Remove(serviceModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool ServiceModelExists(Guid ID)
            => (_context.Service?.Any(s => s.ID == ID)).GetValueOrDefault();
    }
}