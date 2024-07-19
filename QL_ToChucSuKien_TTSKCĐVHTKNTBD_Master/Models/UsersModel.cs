using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class UsersModel
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên nhân viên là bắt buộc.")]
        [Display(Name = "Họ tên")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [Display(Name = "Email")]
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;
        [Display(Name = "Hình ảnh")]
        public string ProfilePicture { get; set; } =  "default-profile.png";
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số.")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Display(Name = "Chọn ảnh")]
        [NotMapped]
        public IFormFile? FrontImg { get; set; }
        // Khóa ngoại tới Role
        [Display(Name = "Chức vụ")]
        public int RoleId { get; set; }
        public RolesModel ? Role { get; set; }
   
    }
}
