using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return View(await _context.Events.ToListAsync());
        }
        public IActionResult Details(int eventId)
        {
            // Truy vấn để lấy thông tin EventRegistration với Event và Customer tương ứng
            var registrationDetails =  _context.EventRegistrations
                .Include(er => er.Event)
                .Include(er => er.Customers)
                .FirstOrDefaultAsync(er => er.EventId == eventId);


            if (registrationDetails == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về 404 Not Found
            }

            // Trả về view hoặc partial view chứa thông tin chi tiết
            return View(registrationDetails); // Chuyển hướng đến view chi tiết sự kiện đăng ký
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
            options.Insert(0, new SelectListItem { Value = "", Text = "-- Thêm địa điểm --" });

            return options;
        }

    }
}
