using Microsoft.EntityFrameworkCore;
using terrain.Models;

namespace terrain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Terrain> Terrains { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ReservationDate> ReservationDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Terrain)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TerrainId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationDate)
                .WithMany()
                .HasForeignKey(r => r.ReservationDateId);

            modelBuilder.Entity<ReservationDate>()
                .HasOne(rd => rd.Terrain)
                .WithMany()
                .HasForeignKey(rd => rd.TerrainId);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Manager>().ToTable("Managers");
            modelBuilder.Entity<Terrain>().ToTable("Terrains");

            // Seed initial manager
            modelBuilder.Entity<Manager>().HasData(new Manager
            {
                Id = 1,
                Nom = "initialManager",
                Password = BCrypt.Net.BCrypt.HashPassword("initialPassword")
            });
        }

    }
}
