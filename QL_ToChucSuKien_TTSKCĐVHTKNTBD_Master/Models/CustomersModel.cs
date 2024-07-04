using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class CustomersModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public ICollection<EventRegistrationModel> Registrations { get; set; } = new List<EventRegistrationModel>();
    }
}
