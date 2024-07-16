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
using Microsoft.AspNetCore.Identity;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers;
using System.Drawing.Printing;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
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
        [Authorize(Roles = "Trưởng phòng")]
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
        [Authorize(Roles = "Trưởng phòng")]
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
        [Authorize(Roles = "Trưởng phòng")]
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
        public async Task<IActionResult> Statistics(DateTime? StartDate, DateTime? EndDate, int? page, bool isLoadMore = false)
        {
            int pageSize = 10; // Số lượng mục trên mỗi trang

            // Query các sự kiện từ database
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

            // Lấy danh sách sự kiện đã sắp xếp theo ngày bắt đầu
            var events = await eventsQuery
                .Include(e => e.Registrations)
                .ThenInclude(r => r.Customers)
                .OrderBy(e => e.EventStartDate)
                .ToPagedListAsync(page ?? 1, pageSize); // Phân trang kết quả

            // Chuyển đổi danh sách sự kiện sang EventDetailViewModel
            var eventDetailsViewModel = events.Select(e => new EventDetailViewModel
            {
                EventName = e.EventName,
                EventStartDate = e.EventStartDate,
                EventEndDate = e.EventEndDate,
                EventLocation = e.EventLocation,
                EventDescription = e.EventDescription,
                ParticipantCount = e.Registrations.Count
            }).ToPagedList(page ?? 1, pageSize);
            if (isLoadMore)
            {
                // Nếu là yêu cầu tải thêm dữ liệu, trả về PartialView chứa danh sách sự kiện
                return PartialView("_EventDetailsPartial", eventDetailsViewModel);
            }
            // Tạo ViewModel cho view chính
            var viewModel = new EventStatisticsViewModel
            {
                StartDate = StartDate,
                EndDate = EndDate,
                EventDetails = eventDetailsViewModel // Gán danh sách sự kiện phân trang
            };

           

            return View(viewModel); // Trả về view chính với dữ liệu đã phân trang
        }






        // Xuất file Execel với Danh sách sự kiện trong tháng lọc
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            // Lấy dữ liệu sự kiện từ Model
            var events = GetFilteredEvents(startDate, endDate);

            // Tạo package Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Tạo sheet Excel
                var sheet = package.Workbook.Worksheets.Add("Events");

                // Định dạng tiêu đề
                sheet.Cells["A1"].Value = "Danh sách sự kiện từ";
                sheet.Cells["A2"].Value = startDate?.ToString("dd/MM/yyyy") ?? "Không xác định";
                sheet.Cells["B1"].Value = "đến";
                sheet.Cells["B2"].Value = endDate?.ToString("dd/MM/yyyy") ?? "Không xác định";

                sheet.Cells["A4"].Value = "Tên sự kiện";
                sheet.Cells["B4"].Value = "Ngày bắt đầu";
                sheet.Cells["C4"].Value = "Ngày kết thúc";
                sheet.Cells["D4"].Value = "Địa điểm";
                sheet.Cells["E4"].Value = "Mô tả";
                sheet.Cells["F4"].Value = "Số người tham gia";

                // Dữ liệu từ Model
                var row = 5;
                foreach (var item in events)
                {
                    sheet.Cells[string.Format("A{0}", row)].Value = item.EventName;
                    sheet.Cells[string.Format("B{0}", row)].Value = item.EventStartDate.ToString("dd/MM/yyyy HH:mm");
                    sheet.Cells[string.Format("C{0}", row)].Value = item.EventEndDate.ToString("dd/MM/yyyy HH:mm");
                    sheet.Cells[string.Format("D{0}", row)].Value = item.EventLocation;
                    sheet.Cells[string.Format("E{0}", row)].Value = item.EventDescription;
                    sheet.Cells[string.Format("F{0}", row)].Value = item.ParticipantCount;

                    row++;
                }

                // Format cho sheet Excel (tùy chọn)

                // Lưu file Excel
                byte[] excelBytes = package.GetAsByteArray();
                string fileName = "DanhSachSuKienTrongThang.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
        // Xuất file PDF với báo cáo các sự kiện trong tháng
        public IActionResult GenerateReport(DateTime? startDate, DateTime? endDate)
        {
            var events = GetFilteredEvents(startDate, endDate);

            // Lấy tên tháng từ startDate (ví dụ: Tháng 7)
            string filteredMonth = startDate?.ToString("MMMM yyyy") ?? "Tất cả";

            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Sử dụng font Unicode nhúng vào tài liệu
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            Font titleFont = new Font(baseFont, 12f, Font.BOLD);
            Font cellFont = new Font(baseFont, 10f);

            document.Open();

            string reportTitle = $"Báo cáo sự kiện {filteredMonth}";
            Paragraph header = new Paragraph(reportTitle, titleFont);
            header.Alignment = Element.ALIGN_CENTER;
            document.Add(header);

            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 3f, 2f, 2f, 3f, 4f, 2f });

            // Add table headers
            table.AddCell(new PdfPCell(new Phrase("Tên sự kiện", titleFont)));
            table.AddCell(new PdfPCell(new Phrase("Ngày bắt đầu", titleFont)));
            table.AddCell(new PdfPCell(new Phrase("Ngày kết thúc", titleFont)));
            table.AddCell(new PdfPCell(new Phrase("Địa điểm", titleFont)));
            table.AddCell(new PdfPCell(new Phrase("Mô tả", titleFont)));
            table.AddCell(new PdfPCell(new Phrase("Số người tham gia", titleFont)));

            // Add data from Model
            foreach (var item in events)
            {
                table.AddCell(new PdfPCell(new Phrase(item.EventName, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(item.EventStartDate.ToString("dd/MM/yyyy HH:mm"), cellFont)));
                table.AddCell(new PdfPCell(new Phrase(item.EventEndDate.ToString("dd/MM/yyyy HH:mm"), cellFont)));
                table.AddCell(new PdfPCell(new Phrase(item.EventLocation, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(item.EventDescription, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(item.ParticipantCount.ToString(), cellFont)));
            }

            document.Add(table);
            document.Close();

            // Return the PDF file
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return File(bytes, "application/pdf", $"Baocaosukien_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");
        }

        // Xuất file Execel với Danh sách người tham gia sự kiện
        public IActionResult ExportParticipants(int eventId)
        {
            // Lấy thông tin sự kiện đăng ký từ database
            var eventDetail =_context.Events?
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
                worksheet.Cells["A1"].Value = "Tên sự kiện";
                worksheet.Cells["B1"].Value = "Ngày tổ chức";
                worksheet.Cells["C1"].Value = "Ngày kết thúc";
                worksheet.Cells["D1"].Value = "Địa điểm";
                worksheet.Cells["E1"].Value = "Mô tả";
                worksheet.Cells["F1"].Value = "Người tham gia";
                worksheet.Cells["G1"].Value = "Email";
                worksheet.Cells["H1"].Value = "Số điện thoại";


                // Đổ dữ liệu vào từng dòng
                int row = 2;
                if(registrations == null)
                {
                    return BadRequest();
                }
                foreach (var registration in registrations)
                {
                    worksheet.Cells[$"A{row}"].Value = registration?.Event?.EventName;
                    worksheet.Cells[$"B{row}"].Value = registration?.Event?.EventStartDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[$"C{row}"].Value = registration?.Event?.EventEndDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[$"D{row}"].Value = registration?.Event?.EventLocation;
                    worksheet.Cells[$"E{row}"].Value = registration?.Event?.EventDescription;
                    worksheet.Cells[$"F{row}"].Value = registration?.Customers?.CustomerName;
                    worksheet.Cells[$"G{row}"].Value = registration?.Customers?.CustomerEmail;
                    worksheet.Cells[$"H{row}"].Value = registration?.Customers?.CustomerPhone;
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
