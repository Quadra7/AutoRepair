using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AutoRepair.Data;
using AutoRepair.Areas.user.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.User.Controllers
{
    [Area("user")]
    public class HomeController : Controller
    {
        ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return Redirect("/");
        }

        // GET: /login
        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginModel loginData, string returnUrl)
        {
            if (string.IsNullOrEmpty(loginData.Email) || string.IsNullOrEmpty(loginData.Password))
                return RedirectToAction("UserNotFound");

            UserModel? user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (user is null) 
                return RedirectToAction("UserNotFound");

            ClaimsIdentity? identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Email, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));

            var claimsPrincipal = new ClaimsPrincipal(identity);

            AuthenticationProperties authProperties;
            if (loginData.rememberMe)
            {
                authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.Now.AddMonths(1),
                    IsPersistent = true,
                };
            }
            else
            {
                authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.AddHours(6),
                    IsPersistent = true,
                };
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsPrincipal), authProperties);

            return !string.IsNullOrEmpty(returnUrl) ? LocalRedirect(returnUrl) : Redirect("/Home");
        }

        // GET: /logout
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Home");
        }

        [Route("/notfound")]
        public IActionResult UserNotFound()
        {
            return View();
        }

        [Route("/accessdenied")]
        public IActionResult UserAccessDenied()
        {
            return View();
        }
    }
}