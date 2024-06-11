using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace terrain.Models
{
    public class User
    {
        public int Id { get; set; } = 0;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Increase size to accommodate BCrypt hash
        public string Password { get; set; } = string.Empty;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
