using Microsoft.EntityFrameworkCore;
using ShortUrl.Repository.Entities;

namespace ShortUrl.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UrlInfoEntity> UrlInfo { get; set; }
    }
}
