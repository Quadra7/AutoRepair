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
    public class AccountantsController : Controller
    {
        private readonly ApplicationContext _context;

        public AccountantsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Accountants
        public async Task<IActionResult> Index()
        {
            var accountants = _context.Accountant
                .Include(a => a.Info)
                .Where(a => a.isActive);

            return accountants != null ?
                View(accountants) :
                Problem("Entity set 'ApplicationContext.Accountants' is null.");
        }

        // GET: Accountants/Details
        public async Task<IActionResult> Details(Guid? ID)
        {
            if (ID == null || _context.Accountant == null)
                return NotFound();

            var accountantModel = await _context.Accountant
                .Include(a => a.Info)
                .FirstOrDefaultAsync(a => a.ID == ID);

            return accountantModel != null ? View(accountantModel) : NotFound();
        }

        // GET: Accountants/Add
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

            return View();
        }

        // POST: Accountants/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid? ID)
        {
            if (_context.Client == null)
                return Problem("Entity set 'ApplicationContext.Clients' is null.");

            var clientModel = await _context.Client
                .Include(c => c.Info)
                .FirstOrDefaultAsync(c => c.ID == ID);

            if (clientModel != null)
            {
                clientModel.isActive = false;

                RoleModel accountantRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "accountant");

                UserModel userModel = await _context.User.FirstOrDefaultAsync(u => u.ID == clientModel.UserID);
                userModel.RoleID = accountantRole.ID;

                AccountantModel accountantModel = await _context.Accountant.FirstOrDefaultAsync(a => a.UserID == userModel.ID);

                if (accountantModel != null)
                {
                    accountantModel.isActive = true;
                    _context.Update(accountantModel);
                }
                else
                {
                    AccountantModel newAccountantModel = new AccountantModel
                    {
                        ID = new Guid(),
                        InfoID = clientModel.InfoID,
                        isActive = true,
                        UserID = clientModel.UserID
                    };
                    _context.Accountant.Add(newAccountantModel);
                }

                _context.Update(userModel);
                _context.Client.Update(clientModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // GET: Accountants/Edit
        public async Task<IActionResult> Edit(Guid? ID)
        {
            if (ID == null || _context.Accountant == null)
                return NotFound();

            var accountantModel = await _context.Accountant
                .Include(a => a.Info)
                .FirstOrDefaultAsync(a => a.ID == ID);

            AccountantEditModel accountantEditModel = new AccountantEditModel
            {
                ID = accountantModel.ID,
                InfoID = accountantModel.InfoID,
                LastName = accountantModel.Info.LastName,
                FirstName = accountantModel.Info.FirstName,
                MiddleName = accountantModel.Info.MiddleName,
                PhoneNumber = accountantModel.Info.PhoneNumber,
                Address = accountantModel.Info.Address,
                UserID= accountantModel.UserID
            };

            return accountantModel != null ? View(accountantEditModel) : NotFound();
        }

        //POST: Accountants/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid ID,
            [Bind("ID,InfoID,LastName,FirstName,MiddleName,PhoneNumber,Address,UserID")] AccountantEditModel editedModel)
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
                    Address = editedModel.Address
                };

                try
                {
                    _context.Info.Update(infoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountantModelExist(editedModel.ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            return View(editedModel);
        }

        // GET: Accountants/Delete
        public async Task<IActionResult> Delete(Guid? ID)
        {
            if (ID == null || _context.Accountant == null)
                return NotFound();

            var accountantModel = await _context.Accountant
                .Include(a => a.Info)
                .FirstOrDefaultAsync(a => a.ID == ID);

            return accountantModel != null ? View(accountantModel) : NotFound();
        }

        // POST: Accountants/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            if (_context.Accountant == null)
                return Problem("Entity set 'ApplicationContext.Accountants' is null.");

            var accountantModel = await _context.Accountant
                .Include(a => a.Info)
                .FirstOrDefaultAsync(a => a.ID == ID);

            if (accountantModel != null)
            {
                accountantModel.isActive = false;

                RoleModel clientRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "client");

                UserModel userModel = await _context.User.FirstOrDefaultAsync(u => u.ID == accountantModel.UserID);
                userModel.RoleID = clientRole.ID;

                ClientModel clientModel = await _context.Client.FirstOrDefaultAsync(u => u.UserID == userModel.ID);
                clientModel.isActive = true;
                
                _context.Update(userModel);
                _context.Accountant.Update(accountantModel);
                _context.Client.Update(clientModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private bool AccountantModelExist(Guid ID)
            => (_context.Accountant?.Any(e => e.ID == ID)).GetValueOrDefault();
    }
}