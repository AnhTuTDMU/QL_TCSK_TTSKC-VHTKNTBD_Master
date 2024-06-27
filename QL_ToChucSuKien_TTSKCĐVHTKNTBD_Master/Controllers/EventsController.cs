using Microsoft.AspNetCore.Mvc;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Events/Detail
        public IActionResult Detail(int eventId)
        {
            var eventDetail = _context.Events.FirstOrDefault(e => e.EventID == eventId);

            if (eventDetail == null)
            {
                return NotFound();
            }          
            return View(eventDetail);
        }
        [HttpGet]
        public IActionResult Register(int eventId)
        {
            var eventDetail = _context.Events.Find(eventId);
            if (eventDetail == null)
            {
                return NotFound();
            }

            var model = new EventRegistrationViewModel
            {
                EventId = eventDetail.EventID,
                EventName = eventDetail.EventName,
                ImgUrl = eventDetail.ImgUrl,
            };

            return PartialView("Register", model);
        }


    }

}
