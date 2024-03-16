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


        // TODO: поменять для использования на localhost
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Server=localhost;Username=postgres;Password=postgres;Database=postgres");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatFile>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            base.OnModelCreating(modelBuilder);
        }
    }
}