using Microsoft.EntityFrameworkCore;
using timetable.Models;

namespace timetable.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>()
                .Property(p => p.LoginId)
                .ValueGeneratedOnAdd();
        }
        public DbSet<Login> Logins { get; set; }
    }
}