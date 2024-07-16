using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using System.Security.Claims;
using System.Text;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.ViewModels;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Security.Cryptography;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
  
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                                    .Include(u => u.Role) // Nạp thông tin Role
                                    .SingleOrDefault(u => u.UserEmail == model.Email);

                if (user != null && VerifyPassword(model.Password, user.Password))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                 new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

                    // Lưu tên người dùng vào Session
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetInt32("UserId", user.UserId);

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không có Email tài khoản này");
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu không hợp lệ");
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

    }
}
