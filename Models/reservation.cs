using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace terrain.Models
{
    public class Reservation
    {
        public int Id { get; set; } = 0;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime HeureDebut { get; set; }

        [ForeignKey("Terrain")]
        public int TerrainId { get; set; }

        [Required]
        public Terrain Terrain { get; set; } = null!;  // Ensuring non-null initialization

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;  // Ensuring non-null initialization
    }
}
