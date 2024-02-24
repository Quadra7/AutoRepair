using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoRepair.Data;
using AutoRepair.Data.Entities;
using NuGet.Protocol;

namespace AutoRepair.Areas.profile.Controllers
{
    [Area("profile")]
    [Authorize(Roles = "client")]
    public class CarController : Controller
    {
        private readonly ApplicationContext _context;

        public CarController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: /Car/GetCars
        public async Task<string> GetCars(bool status, Guid ID)
        {
            var cars = _context.Car.Where(c => c.OwnerID == ID && c.isActive == status);

            return cars.ToJson();
        }

        // GET: /Car/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Car/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CarModel carModel)
        {
            var identities = HttpContext.User.Claims;
            var emailClaim = identities.FirstOrDefault(p => p.Type == ClaimTypes.Email);
            string email = emailClaim.Value;

            var clientModel = await _context.Client
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Email == email);

            carModel.OwnerID = clientModel.ID;

            _context.Car.Add(carModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Car/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Car == null)
                return NotFound();

            var carModel = await _context.Car.FindAsync(ID);

            if (carModel == null)
                return NotFound();

            return View(carModel);
        }

        // POST: /Car/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID, [Bind("ID,Name,Color,LicensePlate,isActive,OwnerID")] CarModel carModel)
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
                    if (!CarModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Car/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Car == null)
                return NotFound();

            var carModel = await _context.Car
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.ID == ID);

            return carModel != null ? View(carModel) : NotFound();
        }

        // POST: /Car/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHorseConfirmed(Guid ID)
        {
            if (_context.Car == null)
                return Problem("Entity set 'ApplicationContext.Cars' is null.");

            var carModel = await _context.Car.FindAsync(ID);

            if (carModel != null)
            {
                carModel.isActive = false;
                _context.Car.Update(carModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool CarModelExists(Guid ID)
            => (_context.Car?.Any(e => e.ID == ID)).GetValueOrDefault();
    }
}
