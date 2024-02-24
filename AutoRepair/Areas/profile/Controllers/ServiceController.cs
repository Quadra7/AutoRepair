using AutoRepair.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace AutoRepair.Areas.profile.Controllers
{
    [Area("profile")]
    [Authorize(Roles = "client")]
    public class ServiceController : Controller
    {
        private readonly ApplicationContext _context;

        public ServiceController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<string> GetServices(Guid masterID)
        {
            var master = await _context.Master.FindAsync(masterID);
            var services = _context.Service.Where(s => s.SpecialtyID == master.SpecialtyID);

            return services.ToJson();
        }
    }
}
