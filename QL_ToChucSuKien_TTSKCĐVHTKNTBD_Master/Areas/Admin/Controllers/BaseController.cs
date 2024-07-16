using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = _context.Users
                                   .Include(u => u.Role)
                                   .FirstOrDefault(u => u.UserId == userId);
                ViewBag.UserName = user?.UserName;
                ViewBag.UserAvatar = user?.ProfilePicture;
            }

            base.OnActionExecuting(context);
        }
    }
}
