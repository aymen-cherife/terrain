using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace terrain.Models
{
    public class Manager
    {
        public int Id { get; set; } = 0;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; } = string.Empty;
    }
}
