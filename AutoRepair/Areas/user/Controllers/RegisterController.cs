using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoRepair.Areas.user.Models;
using AutoRepair.Data;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.user.Controllers
{
    [Area("user")]
    public class RegisterController : Controller
    {
        private readonly ApplicationContext _context;

        public RegisterController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: /index
        [AllowAnonymous]
        [Route("/register")]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /index
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/register")]
        public async Task<IActionResult> Index(
            [Bind("LastName,FirstName,MiddleName,Email,PhoneNumber,Address,Password")] RegisterClientModel regC)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            if (_context.User.Where(p => p.Email == regC.Email).Any())
                return View("UserAlreadyRegistered");

            var uRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "Client");

            UserModel user = new UserModel
            {
                ID = Guid.NewGuid(),
                Email = regC.Email,
                Password = regC.Password,
                RoleID = uRole.ID,
            };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            InfoModel infoModel = new InfoModel
            {
                ID = Guid.NewGuid(),
                LastName = regC.LastName,
                FirstName = regC.FirstName,
                MiddleName = regC.MiddleName,
                PhoneNumber = regC.PhoneNumber,
                Address = regC.Address
            };
            _context.Info.Add(infoModel);
            await _context.SaveChangesAsync();

            ClientModel userClient = new ClientModel
            {
                ID = Guid.NewGuid(),
                InfoID = infoModel.ID,
                isInLoyalProgram = false,
                UserID = user.ID,
            };
            _context.Client.Add(userClient);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}