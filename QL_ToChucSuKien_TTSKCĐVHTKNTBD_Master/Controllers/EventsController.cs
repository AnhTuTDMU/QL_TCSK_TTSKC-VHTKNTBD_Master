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

        // GET: /Events/Detail
        public IActionResult Detail(int eventId)
        {
            var eventDetail = _context.Events.FirstOrDefault(e => e.EventID == eventId);

            if (eventDetail == null)
            {
                return NotFound();
            }
            // Đếm số lượng đăng ký cho sự kiện này
            eventDetail.NumberRegistrations = _context.EventRegistrations
                                             .Count(r => r.EventId == eventId);
            return View(eventDetail);
        }
     

        public async Task<IActionResult> Register(int eventId)
        {
            var eventModel = await _context.Events
                                           .Include(e => e.Registrations) // Bao gồm thông tin đăng ký
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
                        ModelState.AddModelError("", "Event not found.");
                        return Json(new { success = false, message = "Event not found." });
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
                    registrationModel.RegistrationDate = DateTime.Now;
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
