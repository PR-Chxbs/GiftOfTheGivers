using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Controllers
{
    [Authorize] // restrict to logged-in users with "User" role
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() 
        {
            return View();
        }

        // -------- Events (read only) --------
        public async Task<IActionResult> Events()
        {
            var eventsList = await _context.Events.ToListAsync();
            return View(eventsList);
        }

        // -------- Donations (create only) --------
        public IActionResult CreateDonation()
        {
            ViewBag.Events = _context.Events.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonation(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(donation);
            }

            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events)); // redirect back to events
        }

        // -------- Resources (create only) --------
        public IActionResult CreateResource()
        {
            ViewBag.Events = _context.Events.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateResource(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(resource);
            }

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events));
        }
    }
}
