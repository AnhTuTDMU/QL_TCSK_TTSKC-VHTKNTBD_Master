using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Areas.Admin.Controllers;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;
using System.Globalization;

var supportedCultures = new[] { new CultureInfo("vi-VN") };
var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QL_ToChucSuKien_TTSKCĐVHTKNTBD_MasterConnection")));

// Thêm các dịch vụ vào container
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    /*options.IdleTimeout = TimeSpan.FromSeconds(10);*/ // Thời gian timeout ngắn để dễ test
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cookie session là bắt buộc
});

// Cấu hình Authentication và Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10); // Thời gian tồn tại của cookie
        options.LoginPath = "/Account/Login";
        options.LoginPath = "/Account/Logout";// Đường dẫn đến trang đăng nhập
    });
// Cấu hình cho phép yêu cầu Ajax từ các miền khác
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
// Cấu hình Request Localization
builder.Services.AddLocalization();

var app = builder.Build();

// Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts(); 
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Cấu hình Routing
app.MapControllerRoute(
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
