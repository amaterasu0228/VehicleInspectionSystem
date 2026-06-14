using AppointmentService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
    }
}