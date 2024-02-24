using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Data;
using AutoRepair.Data.Entities;
using AutoRepair.Areas.admin.Models;

namespace AutoRepair.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class MastersController : Controller
    {
        private readonly ApplicationContext _context;

        public MastersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Masters
        public async Task<IActionResult> Index()
        {
            var masters = _context.Master
                .Include(m => m.Info)
                .Include(m => m.Specialty)
                .Where(m => m.isActive);

            return masters != null ?
                View(await masters.ToListAsync()) :
                Problem("Entity set 'ApplicationContext.Masters' is null.");
        }

        // GET: Masters/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Master == null)
                return NotFound();

            var masterModel = await _context.Master
                .Include(m => m.Info)
                .Include(m => m.Specialty)
                .FirstOrDefaultAsync(m => m.ID == ID);

            return masterModel != null ? View(masterModel) : NotFound();
        }

        // GET: Masters/Add
        public IActionResult Add()
        {
            var clients = _context.Client
                .Include(c => c.Info)
                .Where(c => c.isActive)
                .Select(item => new
                {
                    ID = item.ID,
                    FullName = $"{item.Info.LastName} {item.Info.FirstName} {item.Info.MiddleName}"
                });

            ViewBag.Clients = new SelectList(clients, "ID", "FullName");
            ViewBag.Specialties = new SelectList(_context.Specialty, "ID", "Name");

            return View();
        }

        // POST: Masters/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid ID, Guid SpecialtyID)
        {
            if (_context.Client == null)
                return Problem("Entity set 'ApplicationContext.Clients' is null.");

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            if (clientModel != null)
            {
                clientModel.isActive = false;

                RoleModel masterRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "master");

                UserModel userModel = await _context.User.FirstOrDefaultAsync(u => u.ID == clientModel.UserID);
                userModel.RoleID = masterRole.ID;

                MasterModel masterModel = await _context.Master.FirstOrDefaultAsync(m => m.UserID == userModel.ID);

                if (masterModel != null)
                {
                    masterModel.isActive = true;
                    _context.Master.Update(masterModel);
                }
                else
                {
                    MasterModel newMasterModel = new MasterModel
                    {
                        ID = new Guid(),
                        InfoID = clientModel.InfoID,
                        SpecialtyID = SpecialtyID,
                        Balance = 0,
                        UserID = clientModel.UserID
                    };

                    _context.Master.Add(newMasterModel);
                }

                _context.Update(userModel);
                _context.Client.Update(clientModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // GET: Masters/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Master == null)
                return NotFound();

            var masterModel = await _context.Master
                .Include(m => m.Info)
                .Include(m => m.Specialty)
                .FirstOrDefaultAsync(m => m.ID == ID);

            MasterEditModel masterEditModel = new MasterEditModel
            {
                ID = masterModel.ID,
                InfoID = masterModel.InfoID,
                LastName = masterModel.Info.LastName,
                FirstName = masterModel.Info.FirstName,
                MiddleName = masterModel.Info.MiddleName,
                PhoneNumber = masterModel.Info.PhoneNumber,
                Address = masterModel.Info.Address,
                SpecialtyID = masterModel.SpecialtyID,
                Balance = masterModel.Balance,
                UserID = masterModel.UserID
            };

            ViewData["SpecialtyID"] = new SelectList(_context.Specialty, "ID", "Name", masterModel.SpecialtyID);

            return masterModel != null ? View(masterEditModel) : NotFound();
        }

        // POST: Masters/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,InfoID,LastName,FirstName,MiddleName,PhoneNumber,Address,SpecialtyID,Balance,UserID")] MasterEditModel editedModel)
        {
            if (editedModel.ID != ID)
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
                    Address = editedModel.Address
                };

                MasterModel masterModel = new MasterModel
                {
                    ID = editedModel.ID,
                    InfoID = editedModel.InfoID,
                    SpecialtyID = editedModel.SpecialtyID,
                    Balance = editedModel.Balance,
                    UserID = editedModel.UserID
                };

                try
                {
                    _context.Info.Update(infoModel);
                    _context.Master.Update(masterModel);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!MasterModelExists(ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            ViewData["SpecialtyID"] = new SelectList(_context.Specialty, "ID", "Name", editedModel.SpecialtyID);

            return View(editedModel);
        }

        // GET: Masters/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Master == null)
                return NotFound();

            var masterModel = await _context.Master
                .Include(m => m.Info)
                .Include(m => m.Specialty)
                .FirstOrDefaultAsync(m => m.ID == ID);

            return masterModel != null ? View(masterModel) : NotFound();
        }

        // POST: Masters/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            if (_context.Master == null)
                return Problem("Entity set 'ApplicationContext.Masters' is null.");

            var masterModel = await _context.Master
                .Include(m => m.Info)
                .Include(m => m.Specialty)
                .FirstOrDefaultAsync(m => m.ID == ID);

            if (masterModel != null)
            {
                masterModel.isActive = false;

                RoleModel clientRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "client");

                UserModel userModel = await _context.User.FirstOrDefaultAsync(u => u.ID == masterModel.UserID);
                userModel.RoleID = clientRole.ID;

                ClientModel clientModel = await _context.Client.FirstOrDefaultAsync(u => u.UserID == userModel.ID);
                clientModel.isActive = true;

                _context.Update(userModel);
                _context.Master.Update(masterModel);
                _context.Client.Update(clientModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private bool MasterModelExists(Guid ID)
            => (_context.Master?.Any(m => m.ID == ID)).GetValueOrDefault();
    }
}
