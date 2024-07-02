using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 4)
        {
            var currentDate = DateTime.Now;
            await AutoUpdateEventStatus();
            var runningEvents = await _context.Events
                .Where(e => e.EventStatus == "1")
                .OrderByDescending(e => e.EventStartDate)
                .AsNoTracking()
                .ToListAsync();

            var upcomingEvents = await _context.Events
                .Where(e => e.EventStatus == "2")
                .OrderBy(e => e.EventStartDate)
                .AsNoTracking()
                .ToListAsync();

            var endedEvents = await _context.Events
                .Where(e => e.EventStatus == "0")
                .OrderByDescending(e => e.EventEndDate)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new EventsViewModel
            {
                RunningEvents = new PaginatedList<EventsModel>(runningEvents, runningEvents.Count, pageNumber, pageSize),
                UpcomingEvents = new PaginatedList<EventsModel>(upcomingEvents, upcomingEvents.Count, pageNumber, pageSize),
                EndedEvents = new PaginatedList<EventsModel>(endedEvents, endedEvents.Count, pageNumber, pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AutoUpdateEventStatus()
        {
            var events = await _context.Events.ToListAsync();
            var currentDate = DateTime.Now;

            foreach (var eventItem in events)
            {
                if (eventItem.EventEndDate < currentDate)
                {
                    eventItem.EventStatus = "0";
                    _context.Update(eventItem);
                }
                else if (eventItem.EventStartDate > currentDate)
                {
                    eventItem.EventStatus = "2";
                    _context.Update(eventItem);
                }
                else
                {
                    eventItem.EventStatus = "1";
                    _context.Update(eventItem);
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Trạng thái sự kiện đã được tự động cập nhật." });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
