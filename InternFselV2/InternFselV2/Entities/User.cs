using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternFselV2.Entities
{
    public class User : Entity
    {       
        [Required]
        [MaxLength(250)]
        public string? FullName { get; set; }
        [Required]
        [MaxLength(250)]
        public string? UserName { get; set; }
        [Required]
        [MaxLength(250)]
        public string? Password { get; set; }
        public ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}
