using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        [Display(Name = "Chọn ảnh")]
        [NotMapped]
        public IFormFile? FrontImg { get; set; }
    }
}
