using System.ComponentModel.DataAnnotations;

namespace InternFselV2.Model.QueryModel
{
    public class LoginQueryModel
    {
        [Required(ErrorMessage = "Nhập đầy đủ UserName")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Nhập đầy đủ Password")]
        public string? Password { get; set; }
    }
}
