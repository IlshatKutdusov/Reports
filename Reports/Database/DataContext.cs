using Microsoft.EntityFrameworkCore;
using Reports.Entities;

namespace Reports.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        DbSet<File> File { get; set; }
        DbSet<User> User { get; set; }
        DbSet<Report> Report { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<File>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(e => e.Login)
                .IsUnique();

            builder.Entity<Report>()
                .HasIndex(e => e.Name)
                .IsUnique();
        }
    }
}
