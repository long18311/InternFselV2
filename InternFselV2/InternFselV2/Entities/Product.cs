using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternFselV2.Entities
{
    public class Product : Entity
    {
        
        [Required]
        [MaxLength(250)]
        public string? Name { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
        public Guid? CreatedUserId { get; set; }
        public User? CreatedUser { get; set; }
    }
}
