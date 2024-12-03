using InternFselV2.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InternFselV2.Model.EnityModel;

namespace InternFselV2.Model.CommandModel.ProductCmd
{
    public class ProductModel
    {

        public string? Name { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
        public Guid? CreatedUserId { get; set; }
        public UserModel? CreatedUser { get; set; }
    }
}
