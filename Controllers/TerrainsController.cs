using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using terrain.Models;

namespace terrain.Controllers
{

    public class TerrainsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerrainsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View for displaying all terrains
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var terrains = await _context.Terrains.ToListAsync();
            return View(terrains);
        }

        // View for details of a single terrain
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);
            if (terrain == null)
            {
                return NotFound();
            }
            return View(terrain);
        }

        // View for creating a new terrain
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handling creation of a new terrain
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Terrain terrain)
        {
            if (ModelState.IsValid)
            {
                _context.Terrains.Add(terrain);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(terrain);
        }

        // View for editing an existing terrain
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);
            if (terrain == null)
            {
                return NotFound();
            }
            return View(terrain);
        }

        // Handling updates to an existing terrain
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Terrain terrain)
        {
            if (id != terrain.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(terrain).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerrainExists(id))
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
            return View(terrain);
        }

        // View for deleting a terrain
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);
            if (terrain == null)
            {
                return NotFound();
            }
            return View(terrain);
        }

        // Handling deletion of a terrain
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);
            if (terrain != null)
            {
                _context.Terrains.Remove(terrain);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TerrainExists(int id)
        {
            return _context.Terrains.Any(e => e.Id == id);
        }
    }
}
