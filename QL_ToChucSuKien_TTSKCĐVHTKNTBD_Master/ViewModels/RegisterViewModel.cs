using System.ComponentModel.DataAnnotations;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [MaxLength(50, ErrorMessage = "Họ tên không được quá 50 ký tự")]
        [Display(Name = "Họ và tên")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(50, ErrorMessage = "Email không được quá 50 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MaxLength(50, ErrorMessage = "Mật khẩu không được quá 50 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [MaxLength(50, ErrorMessage = "Địa chỉ không được quá 50 ký tự")]
        public string Address { get; set; } = string.Empty;
    }
}
