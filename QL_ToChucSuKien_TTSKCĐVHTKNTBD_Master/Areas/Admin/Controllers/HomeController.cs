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
            return View();
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
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = "")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Username");

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "default" });
            }
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
                .Select(u => new UserProfileViewModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserEmail = u.UserEmail,
                    Address = u.Address,
                    ProfilePicture = u.ProfilePicture,
                    PhoneNumber = u.PhoneNumber
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
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
