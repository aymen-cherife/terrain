using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace terrain.Models
{
    public class Terrain
    {
        public int Id { get; set; } = 0;

        [Required]
        public int Numero { get; set; } = 0;

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Largeur { get; set; } = 0.0;

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Longueur { get; set; } = 0.0;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
