using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace terrain.Models
{
    public class Reservation
    {
        public int Id { get; set; } = 0;

        [ForeignKey("ReservationDate")]
        public int ReservationDateId { get; set; }

        [ValidateNever]
        public ReservationDate ReservationDate { get; set; } = null!;  // Ensuring non-null initialization


        [ForeignKey("Terrain")]
        public int TerrainId { get; set; }

        [ValidateNever]
        public Terrain Terrain { get; set; } = null!;  // Ensuring non-null initialization

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ValidateNever]
        public User User { get; set; } = null!;  // Ensuring non-null initialization
    }

}
