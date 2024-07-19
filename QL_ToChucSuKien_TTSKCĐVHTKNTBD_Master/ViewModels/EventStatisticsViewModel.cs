using X.PagedList;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels
{
    public class EventStatisticsViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<EventDetailViewModel> EventDetails { get; set; }
    }
    public class EventDetailViewModel
    {
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventLocation { get; set; }
        public string EventDescription { get; set; }
        public int ParticipantCount { get; set; }
    }

}
