using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class CustomersModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string ? CustomerName { get; set; }
        public string ? CustomerEmail { get; set; }
        public string ? CustomerPhone { get; set; }

        public ICollection<EventRegistrationModel>? Registrations { get; set; }
    }
}
