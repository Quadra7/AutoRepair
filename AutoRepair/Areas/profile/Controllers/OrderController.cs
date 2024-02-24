using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoRepair.Data;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.profile.Controllers
{
    [Area("profile")]
    [Authorize(Roles = "accountant,client,master")]
    public class OrderController : Controller
    {
        private readonly ApplicationContext _context;

        public OrderController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var orders = _context.Order
                .Include(o => o.Client)
                    .ThenInclude(c => c.Info)
                .Include(o => o.Car)
                .Include(o => o.Master)
                    .ThenInclude(m => m.Info)
                .Include(o => o.Service)
                .Include(o => o.Status)
                .Where(o => o.Status.Name == "Accepted!" || o.Status.Name == "Repairing...");

            return _context.Order != null ?
                View(orders) :
                Problem("Entity set 'ApplicationContext.Orders' is null.");
        }

        // GET: /Orders/Details
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

        // GET: /Orders/Create
        public async Task<IActionResult> Create()
        {
            var identities = HttpContext.User.Claims;
            var emailClaim = identities.FirstOrDefault(p => p.Type == ClaimTypes.Email);
            string email = emailClaim.Value;

            var status = await _context.Status
                .FirstOrDefaultAsync(s => s.Name == "Waiting...");

            var currClient = await _context.Client
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Email == email);

            var cars = _context.Car
                .Include(c => c.Owner)
                .Where(c => c.Owner.ID == currClient.ID && c.isActive);

            var masters = _context.Master
                .Include(m => m.Info)
                .Where(m => m.isActive)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewData["Status"] = status.ID;
            ViewData["Client"] = currClient.ID;
            ViewBag.Cars = new SelectList(cars, "ID", "Name");
            ViewBag.Masters = new SelectList(masters, "ID", "FullName");
            ViewBag.Services = new SelectList(_context.Service, "ID", "Name");

            return View();
        }

        // POST:/Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,ClientID,CarID,MasterID,ServiceID,Date,StatusID,IsPaidOut")] OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                _context.Order.Add(orderModel);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            var masters = _context.Master
                .Include(m => m.Info)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            var cars = _context.Car
                .Include(c => c.Owner)
                .Where(c => c.Owner.ID == orderModel.ClientID);

            ViewData["StatusID"] = orderModel.StatusID;
            ViewData["Client"] = orderModel.ClientID;
            ViewData["CarID"] = new SelectList(cars, "ID", "Name", orderModel.CarID);
            ViewBag["MasterID"] = new SelectList(masters, "ID", "FullName", orderModel.MasterID);
            ViewBag["ServiceID"] = new SelectList(_context.Service, "ID", "Name", orderModel.ServiceID);

            return View();
        }

        // GET: /Order/Delete
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

        // POST: /Order/Delete
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

            return RedirectToAction("Index", "Home");
        }

        // GET: /Order/Accept
        public async Task<IActionResult> Accept(Guid? ID)
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
                .FirstOrDefaultAsync(o => o.ID == ID);

            return order != null ? View(order) : NotFound();
        }

        // POST: /Order/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid ID)
        {
            var orderModel = await _context.Order.FindAsync(ID);

            if (orderModel != null)
                orderModel.StatusID = (await _context.Status.FirstOrDefaultAsync(s => s.Name == "Accepted!")).ID;

            _context.Order.Update(orderModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Order/Reject
        public async Task<IActionResult> Reject(Guid? ID)
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

        // POST: /Order/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid ID)
        {
            var orderModel = await _context.Order.FindAsync(ID);

            if (orderModel != null)
                orderModel.StatusID = (await _context.Status.FirstOrDefaultAsync(s => s.Name == "Rejected")).ID;

            _context.Order.Update(orderModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //POST: /Order/Pay
        [HttpPost]
        public async Task<IActionResult> Pay(Guid ID)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var orderModel = await _context.Order
                        .Include(o => o.Client)
                        .Include(o => o.Master)
                        .Include(o => o.Service)
                        .FirstOrDefaultAsync(o => o.ID == ID);

                    if (orderModel == null)
                        return NotFound();

                    orderModel.IsPaidOut = true;
                    orderModel.Master.Balance += orderModel.Client.isInLoyalProgram ?
                        orderModel.Service.Price / 10000 * 400  :
                        orderModel.Service.Price / 100 * 20;

                    _context.Order.Update(orderModel);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //POST: /Order/Start
        [HttpPost]
        public async Task<IActionResult> Start(Guid ID)
        {
            var orderModel = await _context.Order.FindAsync(ID);

            if (orderModel != null)
                orderModel.StatusID = (await _context.Status.FirstOrDefaultAsync(s => s.Name == "Repairing...")).ID;

            _context.Order.Update(orderModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Order");
        }

        //POST: /Order/Finish
        [HttpPost]
        public async Task<IActionResult> Finish(Guid ID)
        {
            var orderModel = await _context.Order.FindAsync(ID);

            if (orderModel != null)
                orderModel.StatusID = (await _context.Status.FirstOrDefaultAsync(s => s.Name == "Finished")).ID;

            _context.Order.Update(orderModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Order");
        }
    }
}
