using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers;
[Area("Admin")]
[Authorize(Roles = "Quản lý")] 
public class ManageUsersController : BaseController
{
    private readonly ApplicationDbContext _context;
    public ManageUsersController(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    // GET: ManageUsers
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
                                          .Include(u => u.Role)
                                          .Where(u => u.Role.RoleId == 2)
                                          .ToListAsync();
        return View(users);
    }

    // GET: ManageUsers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public IActionResult Create()
    {
        ViewBag.Roles = _context.Roles.ToList(); 
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsersModel user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (user.ProfilePicture == null)
                {
                    user.ProfilePicture = "default-profile.png";
                }
                user.Password = HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm nhân viên thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
        return Json(new { success = false, errors });
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Roles = await _context.Roles.ToListAsync();
        return PartialView(user);
    }

    // POST: ManageUsers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsersModel model)
    {
        if (id != model.UserId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var userToUpdate = await _context.Users.FindAsync(id);
                if (userToUpdate != null)
                {
                    // Chỉ cập nhật RoleId
                    userToUpdate.RoleId = model.RoleId;

                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, userId = model.UserId, message = "Người dùng đã được cập nhật chức vụ thành công." });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ: " + ex.Message);
            }
        }

        return Json(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var usersModel = await _context.Users.FindAsync(id);
        if (usersModel == null)
        {
            return NotFound();
        }

        try
        {
            _context.Users.Remove(usersModel);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sự kiện đã được xóa thành công." });
        }
        catch (Exception ex)
        {
            return Json(new { errors = $"Đã xảy ra lỗi: {ex.Message}" });
        }
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.UserId == id);
    }
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

}
