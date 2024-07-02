using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            await AutoUpdateEventStatus();
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
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
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UsersModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserEmail = u.UserEmail,
                    Address = u.Address,
                    ProfilePicture = u.ProfilePicture,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId,
                    Role = u.Role,

                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            var roleName = user.Role?.RoleName;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UsersModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                user.UserName = model.UserName;
                user.UserEmail = model.UserEmail;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;


                if (model.FrontImg != null)
                {
                    user.ProfilePicture = UploadFile(model.FrontImg);
                }

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }


        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = "";
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
