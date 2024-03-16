using DataManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManager
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatFile> Files { get; set; }
        public DbSet<Selection> Selections { get; set; }
        public DbSet<SelectionParams> SelectionParams { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Server=db;Username=postgres;Password=postgres;Database=postgres");
    }
}