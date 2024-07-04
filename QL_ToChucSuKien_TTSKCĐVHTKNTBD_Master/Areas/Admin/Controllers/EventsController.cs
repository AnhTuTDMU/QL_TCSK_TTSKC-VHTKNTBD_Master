using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;
using static Microsoft.IO.RecyclableMemoryStreamManager;
namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Where(e => e.EventStatus == "1").ToListAsync());
        }
        public async Task<IActionResult> UpcomingEvents()
        {
            var upcomingEvents = await _context.Events .Where(e => e.EventStatus == "2").ToListAsync();

            return View(upcomingEvents);
        }
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

        public async Task<IActionResult> RegistrationList(int eventId, int? pageNumber)
        {
            var eventDetail = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventDetail == null)
            {
                return NotFound();
            }

            // Lấy danh sách đăng ký tham gia sự kiện
            var registrationsQuery = _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.Customers)
                .OrderByDescending(r => r.RegistrationDate);

            var paginatedRegistrations = await PaginatedList<EventRegistrationModel>.CreateAsync(
                registrationsQuery.AsQueryable(),
                pageNumber ?? 1, // Trang hiện tại, mặc định là trang 1 nếu không có pageNumber
                7 // Số lượng đăng ký trên mỗi trang
            );

            ViewBag.EventName = eventDetail.EventName;
            ViewBag.EventId = eventDetail.EventID;

            return View(paginatedRegistrations);
        }


        // GET: Admin/Events/Create
        public IActionResult Create()
        {
            ViewBag.EventLocations = GetEventLocationOptions();
            ViewBag.SelectStatus = GetEventStatusOptions();
            return View();
        }

        // POST: Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventsModel model)
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


        // GET: Admin/Events/Edit/{id}
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

        // POST: Admin/Events/Edit/{id}
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

        // POST: Admin/Events/Delete/{id}
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

            // Thêm một tùy chọn để nhập mới EventLocation
            options.Insert(0, new SelectListItem { Value = "-1", Text = "Thêm địa điểm mới" });

            return options;
        }
        public IActionResult ExportParticipants(int eventId)
        {
            // Lấy thông tin sự kiện đăng ký từ database
            var eventDetail =_context.Events
                   .Include(e => e.Registrations)
                       .ThenInclude(r => r.Customers)
                   .FirstOrDefault(e => e.EventID == eventId);


            if (eventDetail == null)
            {
                return NotFound();
            }
            // Lấy danh sách đăng ký tham gia sự kiện
            var registrations = eventDetail.Registrations?.ToList();

            // Tạo một package Excel
            using (var package = new ExcelPackage())
            {
                // Tạo một worksheet mới
                var worksheet = package.Workbook.Worksheets.Add("DanhSachNguoiThamGia");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Tên sự kiện";
                worksheet.Cells[1, 2].Value = "Ngày tổ chức";
                worksheet.Cells[1, 3].Value = "Ngày kết thúc";
                worksheet.Cells[1, 4].Value = "Địa điểm";
                worksheet.Cells[1, 5].Value = "Mô tả";
                worksheet.Cells[1, 6].Value = "Người tham gia";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Số điện thoại";


                // Đổ dữ liệu vào từng dòng
                int row = 2;
                foreach (var registration in registrations)
                {
                    worksheet.Cells[row, 1].Value = eventDetail.EventName;
                    worksheet.Cells[row, 2].Value = eventDetail.EventStartDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = eventDetail.EventEndDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 4].Value = eventDetail.EventLocation;
                    worksheet.Cells[row, 5].Value = eventDetail.EventDescription;
                    worksheet.Cells[row, 6].Value = registration.Customers.CustomerName;
                    worksheet.Cells[row, 7].Value = registration.Customers.CustomerEmail;
                    worksheet.Cells[row, 8].Value = registration.Customers.CustomerPhone;
                    row++;
                }

                // Lưu file Excel vào một MemoryStream
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Trả về file Excel như một FileResult
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DanhSachNguoiThamGia_Event_{eventId}.xlsx");
            }
        }



    }
}
