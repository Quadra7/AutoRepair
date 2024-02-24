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
    public class CarsController : Controller
    {
        private readonly ApplicationContext _context;

        public CarsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = _context.Car
                .Include(c => c.Owner)
                    .ThenInclude(o => o.Info);

            return cars != null ?
                View(cars) :
                Problem("Entity set 'ApplicationContext.Cars' is null.");
        }

        // GET: Cars/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Car == null)
                return NotFound();

            var carModel = await _context.Car
                .Include(c => c.Owner)
                    .ThenInclude(o => o.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            return carModel != null ? View(carModel) : NotFound();
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            var owners = _context.Client
                .Include(c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewBag.Owners = new SelectList(owners, "ID", "FullName");

            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,Name,Color,LicensePlate,isActive,OwnerID")] CarModel carModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            var owners = _context.Client
                .Include(c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewData["OwnerID"] = new SelectList(owners, "ID", "FullName", carModel.OwnerID);

            return View(carModel);
        }

        // GET: Cars/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Car == null)
                return NotFound();

            var carModel = await _context.Car.FindAsync(ID);

            if (carModel == null)
                return NotFound();

            var owners = _context.Client
                .Include(c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewData["OwnerID"] = new SelectList(owners, "ID", "FullName", carModel.OwnerID);

            return View(carModel);
        }

        // POST: Cars/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,Name,Color,LicensePlate,isActive,OwnerID")] CarModel carModel)
        {
            if (ID != carModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelExists(carModel.ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            var combinedList = _context.Client
                .Include(c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewData["OwnerID"] = new SelectList(combinedList, "ID", "FullName", carModel.OwnerID);

            return View(carModel);
        }

        // GET: Cars/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Car == null)
                return NotFound();

            var carModel = await _context.Car
                .Include(c => c.Owner)
                    .ThenInclude(o => o.Info)
                .FirstOrDefaultAsync(car => car.ID == ID);

            return carModel != null ? View(carModel) : NotFound();
        }

        // POST: Cars/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid ID)
        {
            if (_context.Car == null)
                return Problem("Entity set 'ApplicationContext.Cars' is null.");

            var carModel = await _context.Car.FindAsync(ID);

            if (carModel != null)
                _context.Car.Remove(carModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool CarModelExists(Guid ID)
            => (_context.Car?.Any(c => c.ID == ID)).GetValueOrDefault();
    }
}