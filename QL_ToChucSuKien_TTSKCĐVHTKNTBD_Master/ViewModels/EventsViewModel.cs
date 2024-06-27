using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels
{
    public class EventsViewModel
    {
        public PaginatedList<EventsModel> ? RunningEvents { get; set; }
        public PaginatedList<EventsModel> ? UpcomingEvents { get; set; }
        public PaginatedList<EventsModel> ? EndedEvents { get; set; }
    }
}
