using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Index
        public IActionResult Index()
        {
            // Could show overview: total users, events, donations, resources
            return View();
        }

        // -------- Users Management --------
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            ViewBag.Roles = new List<string> { "Admin", "User" };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            if (!ModelState.IsValid) return View(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new List<string> { "Admin", "User" };
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, ApplicationUser user)
        {
            if (id != user.Id) return NotFound();
            if (!ModelState.IsValid) return View(user);

            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
 
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        // -------- Events Management --------
        public async Task<IActionResult> Events()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event @event)
        {
            if (!ModelState.IsValid) return View(@event);
            

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events));
        }

        // GET: /Admin/EditEvent/5
        public async Task<IActionResult> EditEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, Event ev)
        {
            if (id != ev.EventID) return NotFound();
            if (!ModelState.IsValid) return View(ev);

            _context.Update(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events));
        }

        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Events));
        }

        // -------- Donations Management --------
        public async Task<IActionResult> Donations()
        {
            var donations = await _context.Donations
                .Include(d => d.Event)
                .ToListAsync();
            return View(donations);
        }

        public IActionResult CreateDonation()
        {
            ViewBag.Events = _context.Events.ToListAsync().GetAwaiter().GetResult();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonation(Donation donation)
        {
            if (!ModelState.IsValid) return View(donation);
            

            ViewBag.Events = await _context.Events.ToListAsync();

            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Donations));
        }

        // -------- Donations Management --------
        public async Task<IActionResult> EditDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();

            ViewBag.Events = await _context.Events.ToListAsync();
            return View(donation);
        }

        [HttpPost]
        public async Task<IActionResult> EditDonation(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(donation);
            }

            _context.Update(donation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Donations));
        }

        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Donations));
        }

        // -------- Resources Management --------
        public async Task<IActionResult> Resources()
        {
            var resources = await _context.Resources
                .Include(r => r.Event)
                .ToListAsync();
            return View(resources);
        }

        public IActionResult CreateResource()
        {
            ViewBag.Events = _context.Events.ToListAsync().GetAwaiter().GetResult();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateResource(Resource resource)
        {
            if (!ModelState.IsValid) return View(resource);
            

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Resources));
        }

        public async Task<IActionResult> EditResource(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            ViewBag.Events = await _context.Events.ToListAsync();
            return View(resource);
        }

        [HttpPost]
        public async Task<IActionResult> EditResource(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(resource);
            }

            _context.Update(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Resources));
        }

        public async Task<IActionResult> DeleteResource(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Resources));
        }

        // -------- Assignments Management --------
        public async Task<IActionResult> Assignments()
        {
            var assignments = await _context.Assignments
                .Include(a => a.User)
                .Include(a => a.Event)
                .ToListAsync();
            return View(assignments);
        }

        public async Task<IActionResult> CreateAssignment()
        {

            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Events = await _context.Events.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid) return View(assignment);
            

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assignments));
        }

        public async Task<IActionResult> EditAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Events = await _context.Events.ToListAsync();

            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = await _context.Users.ToListAsync();
                ViewBag.Events = await _context.Events.ToListAsync();
                return View(assignment);
            }

            _context.Update(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assignments));
        }

        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assignments));
        }
    }
}
