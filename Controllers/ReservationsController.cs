using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using terrain.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace terrain.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displaying all reservations
        [HttpGet]
        public async Task<IActionResult> ManagerIndex()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Terrain)
                .Include(r => r.User)
                .Include(r => r.ReservationDate)
                .ToListAsync();

            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.ReservationDates = await _context.ReservationDates.ToListAsync();

            var uniqueDates = _context.ReservationDates
                .Select(d => d.Date.Date)
                .Distinct()
                .ToList();
            ViewBag.UniqueDates = uniqueDates;

            return View("~/Views/Manager/Reservation/index.cshtml", reservations);
        }

        // Handling creation of a new reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerCreate(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                // Ensure the terrain exists
                var terrainExists = await _context.Terrains.AnyAsync(t => t.Id == reservation.TerrainId);
                if (!terrainExists)
                {
                    ModelState.AddModelError("", "The selected terrain does not exist.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Ensure the user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == reservation.UserId);
                if (!userExists)
                {
                    ModelState.AddModelError("", "The selected user does not exist.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Check if the reservation date and time is already reserved
                var existingReservation = await _context.ReservationDates
                    .FirstOrDefaultAsync(rd =>
                        rd.Date.Date == reservation.ReservationDate.Date.Date &&
                        rd.HeureDebut == reservation.ReservationDate.HeureDebut &&
                        rd.TerrainId == reservation.TerrainId);

                if (existingReservation != null)
                {
                    ModelState.AddModelError("", "Selected date and time is already reserved.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Add the reservation date entry
                var reservationDate = new ReservationDate
                {
                    Date = reservation.ReservationDate.Date,
                    HeureDebut = reservation.ReservationDate.HeureDebut,
                    TerrainId = reservation.TerrainId,
                    Status = "Reservé"
                };

                Console.WriteLine("ReservationDate before saving it:");
                Console.WriteLine("ReservationDate: " + reservationDate.Date.ToString("yyyy-MM-dd"));
                Console.WriteLine("HeureDebut: " + reservationDate.HeureDebut.ToString(@"hh\:mm"));
                Console.WriteLine("ReservationDate Status: " + reservationDate.Status);
                Console.WriteLine("ReservationDate TerrainId: " + reservationDate.TerrainId);

                _context.ReservationDates.Add(reservationDate);
                await _context.SaveChangesAsync();

                // Link the reservation to the new reservation date
                reservation.ReservationDateId = reservationDate.Id;
                reservation.ReservationDate = reservationDate;  // Ensure the ReservationDate object is correctly linked

                // Add the reservation
                Console.WriteLine("Reservation before saving it:");
                Console.WriteLine("TerrainId: " + reservation.TerrainId);
                Console.WriteLine("UserId: " + reservation.UserId);
                Console.WriteLine("ReservationDate: " + reservation.ReservationDate.Date.ToString("yyyy-MM-dd"));
                Console.WriteLine("HeureDebut: " + reservation.ReservationDate.HeureDebut.ToString(@"hh\:mm"));
                Console.WriteLine("ReservationDate Status: " + reservation.ReservationDate.Status);
                Console.WriteLine("ReservationDate TerrainId: " + reservation.ReservationDate.TerrainId);

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManagerIndex));
            }

            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.ReservationDates = await _context.ReservationDates.ToListAsync();

            return RedirectToAction(nameof(ManagerIndex));
        }

        // Handling updates to an existing reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerEdit(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingReservation = await _context.Reservations
                    .Include(r => r.ReservationDate)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (existingReservation == null)
                {
                    return NotFound();
                }

                // Ensure the terrain exists
                var terrainExists = await _context.Terrains.AnyAsync(t => t.Id == reservation.TerrainId);
                if (!terrainExists)
                {
                    ModelState.AddModelError("", "The selected terrain does not exist.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Ensure the user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == reservation.UserId);
                if (!userExists)
                {
                    ModelState.AddModelError("", "The selected user does not exist.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Check if the new reservation date and time is already reserved
                var existingReservationDate = await _context.ReservationDates
                    .FirstOrDefaultAsync(rd =>
                        rd.Date.Date == reservation.ReservationDate.Date.Date &&
                        rd.HeureDebut == reservation.ReservationDate.HeureDebut &&
                        rd.TerrainId == reservation.TerrainId &&
                        rd.Id != existingReservation.ReservationDateId);

                if (existingReservationDate != null)
                {
                    ModelState.AddModelError("", "Selected date and time is already reserved.");
                    return RedirectToAction(nameof(ManagerIndex));
                }

                // Update the reservation details
                existingReservation.TerrainId = reservation.TerrainId;
                existingReservation.UserId = reservation.UserId;

                // Update the ReservationDate if it's changed
                if (existingReservation.ReservationDate.Date != reservation.ReservationDate.Date ||
                    existingReservation.ReservationDate.HeureDebut != reservation.ReservationDate.HeureDebut)
                {
                    // Remove the old ReservationDate if no other reservations are using it
                    var oldReservationDate = existingReservation.ReservationDate;
                    existingReservation.ReservationDate = new ReservationDate
                    {
                        Date = reservation.ReservationDate.Date,
                        HeureDebut = reservation.ReservationDate.HeureDebut,
                        TerrainId = reservation.TerrainId,
                        Status = "Reservé"
                    };

                    _context.ReservationDates.Add(existingReservation.ReservationDate);

                    var otherReservationsUsingOldDate = await _context.Reservations
                        .CountAsync(r => r.ReservationDateId == oldReservationDate.Id && r.Id != id);

                    if (otherReservationsUsingOldDate == 0)
                    {
                        _context.ReservationDates.Remove(oldReservationDate);
                    }
                }

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
                return RedirectToAction(nameof(ManagerIndex));
            }

            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();
            return RedirectToAction(nameof(ManagerIndex));
        }







        // Handling deletion of a reservation
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ManagerDelete(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ReservationDate)  // Include the ReservationDate
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation != null)
            {
                var reservationDateId = reservation.ReservationDateId;

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                // Check if there are any other reservations using the same ReservationDateId
                var otherReservations = await _context.Reservations
                    .AnyAsync(r => r.ReservationDateId == reservationDateId);

                if (!otherReservations)
                {
                    var reservationDate = await _context.ReservationDates.FindAsync(reservationDateId);
                    if (reservationDate != null)
                    {
                        _context.ReservationDates.Remove(reservationDate);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(ManagerIndex));
        }


        // Displaying all reservations for the logged-in user
        [HttpGet]
        public async Task<IActionResult> UserIndex(string token, int? userId)
        {
            // Try to retrieve user ID from session if not provided in query parameters
            if (userId == null)
            {
                userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "UserAuth");
                }
            }

            Console.WriteLine("This is the user id from session or query params: " + userId);

            var reservations = await _context.Reservations
                .Include(r => r.Terrain)
                .Include(r => r.User)
                .Include(r => r.ReservationDate)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.ReservationDates = await _context.ReservationDates.ToListAsync();

            // Optionally, you can also pass the token to the view if needed
            ViewBag.Token = token;

            return View("~/Views/User/Reservation/index.cshtml", reservations);
        }





        // Handling creation of a new reservation for the logged-in user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(Reservation reservation)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }

            reservation.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                // Ensure the user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == reservation.UserId);
                if (!userExists)
                {
                    ModelState.AddModelError("", "The selected user does not exist.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Ensure the terrain exists
                var terrainExists = await _context.Terrains.AnyAsync(t => t.Id == reservation.TerrainId);
                if (!terrainExists)
                {
                    ModelState.AddModelError("", "The selected terrain does not exist.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Check if the reservation date and time is already reserved
                var existingReservation = await _context.ReservationDates
                    .FirstOrDefaultAsync(rd =>
                        rd.Date.Date == reservation.ReservationDate.Date.Date &&
                        rd.HeureDebut == reservation.ReservationDate.HeureDebut &&
                        rd.TerrainId == reservation.TerrainId);

                if (existingReservation != null)
                {
                    ModelState.AddModelError("", "Selected date and time is already reserved.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Add the reservation date entry
                var reservationDate = new ReservationDate
                {
                    Date = reservation.ReservationDate.Date,
                    HeureDebut = reservation.ReservationDate.HeureDebut,
                    TerrainId = reservation.TerrainId,
                    Status = "Reservé"
                };

                _context.ReservationDates.Add(reservationDate);
                await _context.SaveChangesAsync();

                // Link the reservation to the new reservation date
                reservation.ReservationDateId = reservationDate.Id;
                reservation.ReservationDate = reservationDate;  // Ensure the ReservationDate object is correctly linked

                // Add the reservation
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(UserIndex));
            }

            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.ReservationDates = await _context.ReservationDates.ToListAsync();

            return RedirectToAction(nameof(UserIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(int id, Reservation reservation)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }

            if (id != reservation.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingReservation = await _context.Reservations
                    .Include(r => r.ReservationDate)
                    .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

                if (existingReservation == null)
                {
                    return NotFound();
                }

                // Ensure the terrain exists
                var terrainExists = await _context.Terrains.AnyAsync(t => t.Id == reservation.TerrainId);
                if (!terrainExists)
                {
                    ModelState.AddModelError("", "The selected terrain does not exist.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Ensure the user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    ModelState.AddModelError("", "The selected user does not exist.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Check if the new reservation date and time is already reserved
                var conflictingReservation = await _context.Reservations
                    .Include(r => r.ReservationDate)
                    .FirstOrDefaultAsync(r =>
                        r.ReservationDate.Date == reservation.ReservationDate.Date &&
                        r.ReservationDate.HeureDebut == reservation.ReservationDate.HeureDebut &&
                        r.TerrainId == reservation.TerrainId &&
                        r.Id != reservation.Id);

                if (conflictingReservation != null)
                {
                    ModelState.AddModelError("", "Selected date and time is already reserved.");
                    return RedirectToAction(nameof(UserIndex));
                }

                // Update the existing ReservationDate
                existingReservation.ReservationDate.Date = reservation.ReservationDate.Date;
                existingReservation.ReservationDate.HeureDebut = reservation.ReservationDate.HeureDebut;
                existingReservation.ReservationDate.TerrainId = reservation.TerrainId;

                // Update the reservation details
                existingReservation.TerrainId = reservation.TerrainId;
                existingReservation.UserId = userId.Value;

                _context.Reservations.Update(existingReservation);
                _context.ReservationDates.Update(existingReservation.ReservationDate);

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
                return RedirectToAction(nameof(UserIndex));
            }

            ViewBag.Terrains = await _context.Terrains.ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();
            return RedirectToAction(nameof(UserIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }

            var reservation = await _context.Reservations
                .Include(r => r.ReservationDate)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId.Value);

            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);

            // Remove the reservation date if no other reservations are using it
            var otherReservationsUsingDate = await _context.Reservations
                .CountAsync(r => r.ReservationDateId == reservation.ReservationDateId && r.Id != id);

            if (otherReservationsUsingDate == 0)
            {
                _context.ReservationDates.Remove(reservation.ReservationDate);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserIndex));
        }












        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

    }
}
