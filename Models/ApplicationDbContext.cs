using Microsoft.EntityFrameworkCore;
using terrain.Models; // Adjust this using directive if your models are in a different namespace

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet properties for your entities
    public DbSet<Terrain> terrains { get; set; }
    // Add other DbSets for your models, e.g., Reservations, Users, etc.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // You can configure entity behaviors here
    }
}
