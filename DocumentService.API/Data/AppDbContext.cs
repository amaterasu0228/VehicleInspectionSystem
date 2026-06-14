using DocumentService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DocumentFile> Documents { get; set; }
    }
}