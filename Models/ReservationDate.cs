using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace terrain.Models
{
    public class ReservationDate
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        public string Status { get; set; } = "Disponible"; // "Disponible" or "Reservé"

        [ForeignKey("Terrain")]
        public int TerrainId { get; set; }

        [ValidateNever]
        public Terrain Terrain { get; set; } = null!;
    }
}
