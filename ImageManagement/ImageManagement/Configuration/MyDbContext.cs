using ImageManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace ImageManagement.Configuration
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<ImageEntity> ImageEntity { get; set; }
    }
}