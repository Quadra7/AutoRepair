using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoRepair.Areas.profile.Models;
using AutoRepair.Data;
using AutoRepair.Data.Entities;
using Microsoft.AspNetCore.Authentication;

namespace AutoRepair.Areas.Profile.Controllers
{
    [Area("profile")]
    [Authorize(Roles = "admin,accountant,master,client")]
    public class HomeController : Controller
    {
        ApplicationContext _context;
        private string userRole { get; set; }

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var identities = HttpContext.User.Claims;
            var emailClaim = identities.FirstOrDefault(p => p.Type == ClaimTypes.Email);
            string email = emailClaim.Value;

            // if user signed in as admin
            if (HttpContext.User.IsInRole("admin"))
            {
                UserModel userModel = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

                userRole = "admin";
                ViewBag.role = userRole;

                if (userModel != null)
                    return View("ProfileAdmin", userModel);
            }

            // if user signed in as accountant
            if (HttpContext.User.IsInRole("accountant"))
            {
                var accountantModel = await _context.Accountant
                    .Include(a => a.Info)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.User.Email == email);
                
                userRole = "accountant";
                ViewBag.role = userRole;

                if (accountantModel != null)
                {
                    var orders = await _context.Order.Include(o => o.Client)
                        .Include(o => o.Car)
                        .Include(o => o.Master)
                            .ThenInclude(m => m.Info)
                        .Include(o => o.Service)
                        .Where(o => o.IsPaidOut == false && o.Status.Name == "Finished")
                        .ToListAsync();

                    AccountantViewModel viewModel = new AccountantViewModel
                    {
                        LastName = accountantModel.Info.LastName,
                        FirstName = accountantModel.Info.FirstName,
                        MiddleName = accountantModel.Info.MiddleName,
                        PhoneNumber = accountantModel.Info.PhoneNumber,
                        Address = accountantModel.Info.Address,
                        NotPayedOrders = orders
                    };

                    return View("ProfileAccountant", viewModel);
                }
            }

            // if user signed in as master
            if (HttpContext.User.IsInRole("master"))
            {
                var masterModel = await _context.Master
                    .Include(m => m.Info)
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.User.Email == email);

                userRole = "master";
                ViewBag.role = userRole;

                if (masterModel != null)
                {
                    var orders = await _context.Order
                        .Include(o => o.Client)
                            .ThenInclude(c => c.Info)
                        .Include(o => o.Car)
                        .Include(o => o.Master)
                            .ThenInclude(m => m.Info)
                        .Include(o => o.Service)
                        .Include(o => o.Status)
                        .Where(o => o.MasterID == masterModel.ID && o.Status.Name == "Waiting...")
                        .ToListAsync();

                    MasterViewModel viewModel = new MasterViewModel
                    {
                        LastName = masterModel.Info.LastName,
                        FirstName = masterModel.Info.FirstName,
                        MiddleName = masterModel.Info.MiddleName,
                        PhoneNumber = masterModel.Info.PhoneNumber,
                        Address = masterModel.Info.Address,
                        Balance = masterModel.Balance,
                        Orders = orders
                    };

                    return View("ProfileMaster", viewModel);
                }
            }

            // if user signed in as client
            if (HttpContext.User.IsInRole("client"))
            {
                var clientModel = await _context.Client
                    .Include(c => c.Info)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.User.Email == email);

                if (!clientModel.isActive)
                    return NotFound();

                userRole = "client";
                ViewBag.role = userRole;

                if (clientModel != null)
                {
                    var cars = _context.Car.Where(c => c.OwnerID == clientModel.ID && c.isActive);

                    var orders = _context.Order
                        .Include(o => o.Client)
                            .ThenInclude(c => c.Info)
                        .Include(o => o.Car)
                        .Include(o => o.Master)
                            .ThenInclude(m => m.Info)
                        .Include(o => o.Service)
                        .Include(o => o.Status)
                        .Where(o => o.ClientID == clientModel.ID)
                        .OrderByDescending(o => o.Status.Name.Length);

                    ClientViewModel viewModel = new ClientViewModel
                    {
                        ID = clientModel.ID,
                        LastName = clientModel.Info.LastName,
                        FirstName = clientModel.Info.FirstName,
                        MiddleName = clientModel.Info.MiddleName,
                        PhoneNumber = clientModel.Info.PhoneNumber,
                        Address = clientModel.Info.Address,
                        isInLoyalProgram = clientModel.isInLoyalProgram,
                        Cars = cars,
                        Orders = orders
                    };

                    return View("ProfileClient", viewModel);
                }
            }

            return Unauthorized();
        }
    }
}