using System.ComponentModel.DataAnnotations;
using System.Data;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class EventRegistrationModel
    {
        [Key]
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
        public CustomersModel ? Customers { get; set; }
        public EventsModel ? Event { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
