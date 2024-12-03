using InternFselV2.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternFselV2.Model.CommandModel.UserCmd
{
    public class CreateUserCommandModel
    {
        [Required(ErrorMessage = "Nhập đầy đủ họ tên")]
        [MaxLength(250, ErrorMessage = "Họ và tên không đc quá dài")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Nhập đầy đủ UserName")]
        [MaxLength(250, ErrorMessage = "UserName không đc quá dài")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Nhập đầy đủ mật khẩu")]
        [MaxLength(250, ErrorMessage = "Mật khẩu không đc quá dài")]
        public string? Password { get; set; }

    }
}
