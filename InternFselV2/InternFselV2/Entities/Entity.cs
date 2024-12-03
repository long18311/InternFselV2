using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternFselV2.Entities
{
    public class Entity
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
    }
}
