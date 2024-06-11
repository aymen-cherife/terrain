using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using terrain.Models;

namespace terrain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")] // Restrict access to managers only
    public class TerrainsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TerrainsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Terrains
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Terrain>>> GetTerrains()
        {
            return await _context.Terrains.ToListAsync();
        }

        // GET: api/Terrains/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Terrain>> GetTerrain(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);

            if (terrain == null)
            {
                return NotFound();
            }

            return terrain;
        }

        // POST: api/Terrains
        [HttpPost]
        public async Task<ActionResult<Terrain>> PostTerrain(Terrain terrain)
        {
            _context.Terrains.Add(terrain);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTerrain), new { id = terrain.Id }, terrain);
        }

        // PUT: api/Terrains/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerrain(int id, Terrain terrain)
        {
            if (id != terrain.Id)
            {
                return BadRequest();
            }

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

            return NoContent();
        }

        // DELETE: api/Terrains/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerrain(int id)
        {
            var terrain = await _context.Terrains.FindAsync(id);
            if (terrain == null)
            {
                return NotFound();
            }

            _context.Terrains.Remove(terrain);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TerrainExists(int id)
        {
            return _context.Terrains.Any(e => e.Id == id);
        }
    }
}
