using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {

        }

        public DbSet<UsersModel> Users { get; set; }
        public DbSet<EventsModel> Events { get; set; }
        public DbSet<RolesModel> Roles { get; set; }
        public DbSet<EventRegistrationModel> EventRegistrations { get; set; }
        public DbSet<CustomersModel> Customers { get; set; }
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập quan hệ 1-nhiều giữa UsersModel và Role
            modelBuilder.Entity<UsersModel>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);
      
            // Thiết lập quan hệ nhiều-nhiều giữa Events và EventRegistrations và Customer
            modelBuilder.Entity<EventRegistrationModel>()
                 .HasOne(r => r.Event)
                 .WithMany(e => e.Registrations)
                 .HasForeignKey(r => r.EventId);
            modelBuilder.Entity<EventRegistrationModel>()
                 .HasOne(r => r.Customers)
                 .WithMany(c => c.Registrations)
                 .HasForeignKey(r => r.CustomerId);
            // Seed dữ liệu vào bảng Role
            modelBuilder.Entity<RolesModel>().HasData(
                new RolesModel
                {
                    RoleId = 1,
                    RoleName = "Quản lý",
                    Permissions = new List<string> { "Công việc A", "Công việc C" }
                },
                new RolesModel
                {
                    RoleId = 2,
                    RoleName = "Trưởng phòng",
                    Permissions = new List<string> { "Công việc B", "Công việc D" }
                }
            );
            modelBuilder.Entity<UsersModel>().HasData(
                new UsersModel
                {
                    UserId = 1,
                    UserName = "Nguyễn Anh Tú",
                    UserEmail = "AnhTu080302@gmail.com",
                    Password = HashPassword("AnhTu080302@"),
                    Address = "Bình Dương",
                    PhoneNumber = "0332613703",
                    RoleId = 1,
                }
            );
            modelBuilder.Entity<UsersModel>().HasData(
                new UsersModel
                {
                    UserId = 2,
                    UserName = "Mai Xuân Tiền",
                    UserEmail = "XT123@gmail.com",
                    Password = HashPassword("XuanTien123@"),
                    Address = "Bình Dương",
                    PhoneNumber = "0123456789",
                    RoleId = 2,
                }
            );
        }

    }
}
