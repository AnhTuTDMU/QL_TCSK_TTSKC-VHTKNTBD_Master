using X.PagedList;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels
{
    public class EventStatisticsViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IPagedList<EventDetailViewModel> EventDetails { get; set; }
    }
   
}
