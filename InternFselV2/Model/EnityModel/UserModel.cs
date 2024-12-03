using InternFselV2.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InternFselV2.Model.CommandModel.ProductCmd;

namespace InternFselV2.Model.EnityModel
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }
        public IList<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
