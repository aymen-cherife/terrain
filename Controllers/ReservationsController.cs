using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using terrain.Models;
using System.Security.Claims;

namespace terrain.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View for displaying all reservations
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservations = await _context.Reservations.Where(r => r.UserId == userId).ToListAsync();
            return View(reservations);
        }

        // View for details of a single reservation
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null || reservation.UserId != userId)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // View for creating a new reservation
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handling creation of a new reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            reservation.UserId = userId;

            if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // View for editing an existing reservation
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null || reservation.UserId != userId)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // Handling updates to an existing reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (id != reservation.Id || reservation.UserId != userId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(reservation).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // View for deleting a reservation
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null || reservation.UserId != userId)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // Handling deletion of a reservation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation != null && reservation.UserId == userId)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
