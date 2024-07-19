using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
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
        public async Task<IActionResult> LoadAllRunningEvents(int runningPageNumber = 1, int runningPageSize = 10)
        {
            var runningEventsQuery = _context.Events
               .Where(e => e.EventStatus == "1")
               .OrderByDescending(e => e.EventStartDate)
               .AsQueryable();


            var runningEvents = await PaginatedList<EventsModel>.CreateAsync(runningEventsQuery.AsNoTracking(), runningPageNumber, runningPageSize);

            var viewModel = new EventsViewModel
            {
                RunningEvents = runningEvents,
            };

            return View(viewModel);
        }
        public async Task<IActionResult> LoadAllUpcoming(int upcomingPageNumber = 1, int upcomingPageSize = 10)
        {
            var upcomingEventsQuery = _context.Events
              .Where(e => e.EventStatus == "2")
              .OrderBy(e => e.EventStartDate)
              .AsQueryable();


            var upcomingEvents = await PaginatedList<EventsModel>.CreateAsync(upcomingEventsQuery.AsNoTracking(), upcomingPageNumber, upcomingPageSize);

            var viewModel = new EventsViewModel
            {
                UpcomingEvents = upcomingEvents,
            };

            return View(viewModel);
        }
        public async Task<IActionResult> LoadAllEndedEvents(int endedPageNumber = 1, int endedPageSize = 10)
        {
            var endedEventsQuery = _context.Events
                .Where(e => e.EventStatus == "0")
                .OrderByDescending(e => e.EventEndDate)
                .AsQueryable();

            var endedEvents = await PaginatedList<EventsModel>.CreateAsync(endedEventsQuery.AsNoTracking(), endedPageNumber, endedPageSize);

            var viewModel = new EventsViewModel
            {
                EndedEvents = endedEvents
            };

            return View(viewModel); 
        }

      

        // GET: /Events/Detail
        public async Task<IActionResult> Detail(int eventId)
        {
            var eventDetail = await _context.Events.FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventDetail == null)
            {
                return NotFound();
            }

            // Đếm số lượng đăng ký cho sự kiện này
            eventDetail.NumberRegistrations = await _context.EventRegistrations
                                              .CountAsync(r => r.EventId == eventId);

            return View(eventDetail);
        }

        public async Task<IActionResult> Register(int eventId)
        {
            var eventModel = await _context.Events
                                           .Include(e => e.Registrations)
                                           .FirstOrDefaultAsync(e => e.EventID == eventId);
            if (eventModel == null)
            {
                return NotFound();
            }

            var registrationModel = new EventRegistrationModel
            {
                EventId = eventModel.EventID,
                Event = eventModel,
                Customers = new CustomersModel()
            };

            return View(registrationModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(EventRegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var eventModel = await _context.Events.FindAsync(registrationModel.EventId);
                    if (eventModel == null)
                    {
                        ModelState.AddModelError("", "Sự kiện không có. Lỗi");
                        return Json(new { success = false, message = "Sự kiện không có. Lỗi" });
                    }

                    registrationModel.Event = eventModel;

                    // Tìm khách hàng bằng email
                    var customer = await _context.Customers
                                                 .FirstOrDefaultAsync(c => c.CustomerEmail == registrationModel.Customers.CustomerEmail);

                    if (customer != null)
                    {
                        // Kiểm tra xem khách hàng đã đăng ký sự kiện này chưa
                        var existingRegistration = await _context.EventRegistrations
                            .FirstOrDefaultAsync(r => r.EventId == registrationModel.EventId && r.CustomerId == customer.CustomerId);

                        if (existingRegistration != null)
                        {
                            return Json(new { success = false, message = "Email này đã đăng ký cho sự kiện." });
                        }
                        // Gán CustomerId cho registrationModel
                        registrationModel.CustomerId = customer.CustomerId;
                    }
                    else
                    {
                        // Tạo một khách hàng mới nếu không tìm thấy
                        var newCustomer = new CustomersModel
                        {
                            CustomerName = registrationModel.Customers.CustomerName,
                            CustomerEmail = registrationModel.Customers.CustomerEmail,
                            CustomerPhone = registrationModel.Customers.CustomerPhone
                        };
                        _context.Customers.Add(newCustomer);
                        await _context.SaveChangesAsync();

                        registrationModel.CustomerId = newCustomer.CustomerId;
                    }
                    registrationModel.RegistrationDate = DateTime.UtcNow;

                    // Xóa thông tin khách hàng để tránh lưu trùng lặp vào EventRegistrations
                    registrationModel.Customers = null;

                    // Thêm bản đăng ký vào cơ sở dữ liệu
                    _context.EventRegistrations.Add(registrationModel);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng đến trang thành công hoặc hiển thị thông báo thành công
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, errors });
        }

    }
}
