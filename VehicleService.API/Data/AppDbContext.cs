using Microsoft.EntityFrameworkCore;
using VehicleService.API.Models;

namespace VehicleService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<VehicleInspection> Inspections { get; set; }

        public DbSet<InspectionReminder> InspectionReminders { get; set; }
    }

}