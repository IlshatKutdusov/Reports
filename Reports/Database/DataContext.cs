using Microsoft.EntityFrameworkCore;
using Reports.Models;

namespace Reports.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        DbSet<File> File { get; set; }
        DbSet<User> User { get; set; }
    }
}
