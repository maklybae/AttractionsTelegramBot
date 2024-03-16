using Microsoft.EntityFrameworkCore;

namespace DataManager
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Server=db;Username=postgres;Password=postgres;Database=postgres");
    }
}