using InternFselV2.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InternFselV2.Model.CommandModel.ProductCmd
{
    public class UpdateProductCommandModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "nhập đầy đủ Tên")]
        [MaxLength(250, ErrorMessage = "Tên không đc quá dài")]
        public string? Name { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
    }
}
