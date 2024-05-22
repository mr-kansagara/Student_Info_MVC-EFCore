using InformationOfStudent.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace InformationOfStudent.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Student> Student { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Branch>().HasMany(e => e.Student);


        }

    }
}
