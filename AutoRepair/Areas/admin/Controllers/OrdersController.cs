using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Data;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationContext _context;

        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = _context.Order
                .Include(o => o.Client)
                    .ThenInclude(c => c.Info)
                .Include(o => o.Car)
                .Include(o => o.Master)
                    .ThenInclude(m => m.Info)
                .Include(o => o.Service)
                .Include(o => o.Status);

            return _context.Order != null ?
                View(await orders.ToListAsync()) :
                Problem("Entity set 'ApplicationContext.Orders' is null.");
        }

        // GET: Orders/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Order == null)
                return NotFound();

            var order = await _context.Order
                .Include(o => o.Client)
                    .ThenInclude(c => c.Info)
                .Include(o => o.Car)
                .Include(o => o.Master)
                    .ThenInclude(m => m.Info)
                .Include(o => o.Service)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.ID == ID);

            return order != null ? View(order) : NotFound();
        }

        // GET: Orders/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Order == null)
                return NotFound();

            var orderModel = await _context.Order
                .Include(o => o.Client)
                .Include(o => o.Car)
                    .ThenInclude(c => c.Owner)
                .Include(o => o.Master)
                .Include(o => o.Service)
                .Include(o => o.Status)
                .Where(o => o.Client.ID == o.Car.Owner.ID)
                .FirstOrDefaultAsync(o => o.ID == ID);

            var clients = _context.Client
                .Include (c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            var masters = _context.Master
                .Include(m => m.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewBag.Clients = new SelectList(clients, "ID", "FullName");
            ViewBag.Cars = new SelectList(_context.Car, "ID", "Name");
            ViewBag.Masters = new SelectList(masters, "ID", "FullName");
            ViewBag.Services = new SelectList(_context.Service, "ID", "Name");
            ViewBag.Statuses = new SelectList(_context.Status, "ID", "Name");

            return orderModel != null ? View(orderModel) : NotFound();
        }

        // POST: Orders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,ClientID,CarID,MasterID,ServiceID,Date,StatusID,isPaidOut")] OrderModel orderModel)
        {
            if (ID != orderModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Order.Update(orderModel);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!OrderModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            var clients = _context.Client
                .Include(c => c.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            var masters = _context.Master
                .Include(m => m.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewData["ClientID"] = new SelectList(clients, "ID", "FullName", orderModel.ClientID);
            ViewData["CarID"] = new SelectList(_context.Car, "ID", "Name", orderModel.CarID);
            ViewData["MasterID"] = new SelectList(masters, "ID", "FullName", orderModel.MasterID);
            ViewData["ServiceID"] = new SelectList(_context.Service, "ID", "Name", orderModel.ServiceID);
            ViewData["StatusID"] = new SelectList(_context.Status, "ID", "Name", orderModel.StatusID);

            return View(orderModel);
        }

        // GET: Oders/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Order == null)
                return NotFound();

            var order = await _context.Order
                .Include(o => o.Client)
                    .ThenInclude(c => c.Info)
                .Include(o => o.Car)
                .Include(o => o.Master)
                    .ThenInclude(m => m.Info)
                .Include(o => o.Service)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.ID == ID);

            return order != null ? View(order) : NotFound();
        }

        // POST: Orders/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConformed(Guid ID)
        {
            if (_context.Order == null)
                return Problem("Entity set 'ApplicationContext.Orders' is null.");

            var orderModel = await _context.Order.FindAsync(ID);

            if (orderModel != null)
                _context.Order.Remove(orderModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool OrderModelExists(Guid ID)
            => (_context.Order?.Any(o => o.ID == ID)).GetValueOrDefault();
    }
}
