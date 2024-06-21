using Microsoft.EntityFrameworkCore;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {

        }
        public DbSet<UsersModel> Users { get; set; }
    }
}
