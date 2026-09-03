using Microsoft.EntityFrameworkCore;
using Web_PostgreSQL_TestWork.Models;

namespace Web_PostgreSQL_TestWork.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<BD_Objects> BD_Objects { get; set; }
    }
}