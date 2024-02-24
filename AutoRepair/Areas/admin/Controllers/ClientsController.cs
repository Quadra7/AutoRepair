using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Data;
using AutoRepair.Data.Entities;
using AutoRepair.Areas.admin.Models;

namespace AutoRepair.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class ClientsController : Controller
    {
        private readonly ApplicationContext _context;

        public ClientsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = _context.Client
                .Include(c => c.Info)
                .Where(c => c.isActive); ;

            return clients != null ? 
                View(clients) :
                Problem("Entity set 'ApplicationContext.Clients' is null.");
        }

        // GET: Clients/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Client == null)
                return NotFound();

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            return clientModel != null ? View(clientModel) : NotFound();
        }

        // GET: Clients/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Client == null)
                return NotFound();

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            ClientEditModel clientEditModel = new ClientEditModel
            {
                ID = clientModel.ID,
                InfoID = clientModel.InfoID,
                LastName = clientModel.Info.LastName,
                FirstName = clientModel.Info.FirstName,
                MiddleName = clientModel.Info.MiddleName,
                PhoneNumber = clientModel.Info.PhoneNumber,
                Address = clientModel.Info.Address,
                isInLoyalProgram = clientModel.isInLoyalProgram,
                UserID = clientModel.UserID
            };

            return clientModel != null ? View(clientEditModel) : NotFound();
        }

        // POST: Clients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,InfoID,LastName,FirstName,MiddleName,PhoneNumber,Address,isInLoyalProgram,UserID")] ClientEditModel editedModel)
        {
            if (ID != editedModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                InfoModel infoModel = new InfoModel
                { 
                    ID = editedModel.InfoID,
                    LastName = editedModel.LastName,
                    FirstName = editedModel.FirstName,
                    MiddleName = editedModel.MiddleName,
                    PhoneNumber = editedModel.PhoneNumber,
                    Address = editedModel.Address,
                };

                ClientModel clientModel = new ClientModel
                {
                    ID = editedModel.ID,
                    InfoID = editedModel.InfoID,
                    isInLoyalProgram = editedModel.isInLoyalProgram,
                    UserID = editedModel.UserID
                };

                try
                {
                    _context.Info.Update(infoModel);
                    _context.Client.Update(clientModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            return View(editedModel);
        }

        // GET: Clients/Delete
        public async Task<IActionResult> Delete(Guid ID)
        {
            if (ID == null || _context.Client == null)
                return NotFound();

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            return clientModel != null ? View(clientModel) : NotFound();
        }

        // POST: Clients/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            if (_context.Client == null)
                return Problem("Entity set 'ApplicationContext.Clients' is null.");

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            if (clientModel != null)
            {
                clientModel.isActive = false;
                _context.Client.Update(clientModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private bool ClientModelExists(Guid ID)
            => (_context.Client?.Any(c => c.ID == ID)).GetValueOrDefault();
    }
}
