using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class EventsModel
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Tên sự kiện là bắt buộc.")]
        [Display(Name = "Tên sự kiện")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả sự kiện là bắt buộc.")]
        [Display(Name = "Mô tả sự kiện")]
        public string EventDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày bắt đầu không hợp lệ.")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime EventStartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày kết thúc không hợp lệ.")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EventEndDate { get; set; }

        [Required(ErrorMessage = "Địa điểm là bắt buộc.")]
        [Display(Name = "Địa điểm")]
        public string EventLocation { get; set; } = string.Empty;
        [Display(Name = "Hình")]
        public string ImgUrl { get; set; } = string.Empty;
 
        [Display(Name = "Trạng thái")]
        public string EventStatus { get; set; } = string.Empty;
        [Display(Name = "Chọn ảnh")]
        [NotMapped]
        public IFormFile? FrontImg { get; set; }
        public ICollection<EventRegistrationModel> ? Registrations { get; set; }
        [Display(Name ="Số người tham gia")]
        public int NumberRegistrations { get; set; }

    }
}
