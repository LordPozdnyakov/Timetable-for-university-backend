using Microsoft.EntityFrameworkCore;
using timetable.Models;

namespace timetable.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {}

        public DbSet<User> Users { get; set; }
    }
}