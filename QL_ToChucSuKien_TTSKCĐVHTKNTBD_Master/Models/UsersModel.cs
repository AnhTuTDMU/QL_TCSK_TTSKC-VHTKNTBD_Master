using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class UsersModel
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? FrontImg { get; set; }

    }
}
