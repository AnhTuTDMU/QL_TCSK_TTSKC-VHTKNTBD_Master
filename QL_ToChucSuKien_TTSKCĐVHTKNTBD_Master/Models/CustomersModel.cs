using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class CustomersModel
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email khách hàng là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại khách hàng là bắt buộc.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số.")]
        public string CustomerPhone { get; set; } = string.Empty;

        public ICollection<EventRegistrationModel> Registrations { get; set; } = new List<EventRegistrationModel>();
    }
}
