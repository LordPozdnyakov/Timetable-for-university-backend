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
            modelBuilder.Entity<User>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }

        public DbSet<User> Users { get; set; }
    }
}