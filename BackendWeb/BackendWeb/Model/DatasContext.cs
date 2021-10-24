using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendWeb.Model
{
    public class DatasContext:DbContext
    {
        public DatasContext(DbContextOptions<DatasContext> options):base (options)
        {

        }
        public DbSet<Users> User { get; set; }
    }
}
