using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using Microsoft.AspNetCore.Identity;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using X.PagedList;
namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Areas.Admin.Controllers
{ 
    [Area("Admin")]
   
    public class EventsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
       
        }
        [Authorize(Roles = "Quản lý, Trưởng phòng")]
        // GET: Admin/Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Where(e => e.EventStatus == "1")
                .OrderByDescending(e => e.EventStartDate)
                .ToListAsync();

            return View(events);
        }


        [Authorize(Roles = "Quản lý, Trưởng phòng")]
        public async Task<IActionResult> UpcomingEvents()
        {
            var upcomingEvents = await _context.Events .Where(e => e.EventStatus == "2").ToListAsync();

            return View(upcomingEvents);
        }
        [Authorize(Roles = "Quản lý, Trưởng phòng")]
        public async Task<IActionResult> EndedEvents()
        {
            // Lấy tất cả sự kiện có EventStatus == "0"
            var endedEvents = await _context.Events.Where(e => e.EventStatus == "0").ToListAsync();

            // Đếm số lượng đăng ký cho từng sự kiện
            foreach (var eventDetail in endedEvents)
            {
                eventDetail.NumberRegistrations = await _context.EventRegistrations
                    .CountAsync(r => r.EventId == eventDetail.EventID);

                _context.Update(eventDetail);
            }

            await _context.SaveChangesAsync();

            return View(endedEvents);
        }
        [Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> RegistrationList(int eventId)
        {
            var eventDetail = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventDetail == null)
            {
                return NotFound();
            }

            // Lấy danh sách đăng ký tham gia sự kiện
            var registrations = await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.Customers)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            ViewBag.EventName = eventDetail.EventName;
            ViewBag.EventId = eventDetail.EventID;
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            return View(registrations);
        }


        [Authorize(Roles = "Trưởng phòng")]
        public IActionResult Create()
        {
            ViewBag.EventLocations = GetEventLocationOptions();
            ViewBag.SelectStatus = GetEventStatusOptions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventsModel model, string newLocationInput)
        {
            if (model.EventStartDate <= DateTime.Now)
            {
                ModelState.AddModelError("EventStartDate", "Ngày bắt đầu sự kiện phải lớn hơn ngày hiện tại.");
            }

            if (model.EventEndDate <= DateTime.Now)
            {
                ModelState.AddModelError("EventEndDate", "Ngày kết thúc sự kiện phải lớn hơn ngày hiện tại.");
            }

            if (model.EventEndDate <= model.EventStartDate)
            {
                ModelState.AddModelError("EventEndDate", "Ngày kết thúc sự kiện phải lớn hơn ngày bắt đầu sự kiện.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.FrontImg != null)
                    {
                        string filePath = SaveFile(model.FrontImg);
                        model.ImgUrl = filePath;
                    }

                    if (!string.IsNullOrEmpty(newLocationInput))
                    {
                        // Nếu người dùng đã nhập địa điểm mới
                        model.EventLocation = newLocationInput;
                    }

                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, eventId = model.EventID, message = "Sự kiện đã được tạo thành công." });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                    return Json(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
            }

            return Json(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }


        private string SaveFile(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return "/uploads/" + uniqueFileName;
        }
        [Authorize(Roles = "Trưởng phòng")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventsModel = await _context.Events.FindAsync(id);
            if (eventsModel == null)
            {
                return NotFound();
            }

            ViewBag.EventLocations = GetEventLocationOptions();
            ViewBag.SelectStatus = GetEventStatusOptions();
            return View(eventsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventsModel model)
        {
            if (id != model.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.FrontImg != null)
                    {
                        if (!string.IsNullOrEmpty(model.ImgUrl))
                        {
                            DeleteFile(model.ImgUrl);
                        }
                        // Lưu hình ảnh mới
                        string filePath = SaveFile(model.FrontImg);
                        model.ImgUrl = filePath;
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, eventId = model.EventID, message = "Sự kiện đã được cập nhật thành công." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventModelExists(model.EventID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Json(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        private bool EventModelExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }

        private void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        [Authorize(Roles = "Trưởng phòng")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var eventsModel = await _context.Events.FindAsync(id);
            if (eventsModel == null)
            {
                return NotFound();
            }

            try
            {
                _context.Events.Remove(eventsModel);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sự kiện đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { errors = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
       
        public List<SelectListItem> GetEventStatusOptions()
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Ẩn sự kiện" },
                new SelectListItem { Value = "1", Text = "Đang diễn ra" },
                new SelectListItem { Value = "2", Text = "Sắp diễn ra" },
            };
            return options;
        }
        public List<SelectListItem> GetEventLocationOptions()
        {
            // Lấy danh sách các EventLocation từ dữ liệu
            var locations = _context.Events.Select(e => e.EventLocation).Distinct().ToList();

            // Chuyển đổi danh sách EventLocation thành SelectListItem
            var options = locations.Select(l => new SelectListItem
            {
                Value = l,
                Text = l
            }).ToList();
            return options;
        }
        public async Task<IActionResult> Statistics(DateTime? StartDate, DateTime? EndDate)
        {
            var eventsQuery = _context.Events.AsQueryable();

            if (StartDate.HasValue)
            {
                DateTime startClean = StartDate.Value.Date;
                eventsQuery = eventsQuery.Where(e => e.EventStartDate.Date >= startClean);
            }

            if (EndDate.HasValue)
            {
                DateTime endClean = EndDate.Value.Date;
                eventsQuery = eventsQuery.Where(e => e.EventEndDate.Date <= endClean);
            }

            var events = await eventsQuery
                .Include(e => e.Registrations)
                .ThenInclude(r => r.Customers)
                .ToListAsync();

            var eventDetails = events.Select(e => new EventDetailViewModel
            {
                EventName = e.EventName,
                EventStartDate = e.EventStartDate,
                EventEndDate = e.EventEndDate,
                EventLocation = e.EventLocation,
                EventDescription = e.EventDescription,
                ParticipantCount = e.Registrations.Count
            }).ToList();

            var viewModel = new EventStatisticsViewModel
            {
                StartDate = StartDate,
                EndDate = EndDate,
                EventDetails = eventDetails
            };

            return View(viewModel);
        }



        private IQueryable<EventDetailViewModel> GetFilteredEvents(DateTime? startDate, DateTime? endDate)
        {
            // Lấy dữ liệu sự kiện theo các điều kiện lọc (startDate và endDate)
            IQueryable<EventDetailViewModel> events = _context.Events
                .Where(e => (!startDate.HasValue || e.EventStartDate >= startDate) &&
                            (!endDate.HasValue || e.EventEndDate <= endDate))
                .Select(e => new EventDetailViewModel
                {
                    EventName = e.EventName,
                    EventStartDate = e.EventStartDate,
                    EventEndDate = e.EventEndDate,
                    EventLocation = e.EventLocation,
                    EventDescription = e.EventDescription,
                    ParticipantCount = e.Registrations.Count() 
                });

            return events;
        }
 

    }
}
