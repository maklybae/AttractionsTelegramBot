using DataManager.Mapping;
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
        public DbSet<Sorting> Sortings { get; set; }
        public DbSet<SortingParams> SortingParams { get; set; }

        public DatabaseContext() { }


        // TODO: поменять для использования на localhost
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Server=localhost;Username=postgres;Password=postgres;Database=postgres");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatFile>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            modelBuilder.Entity<Selection>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            modelBuilder.Entity<Sorting>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            modelBuilder.Entity<SortingParams>().Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            modelBuilder.Entity<Chat>().Property(e => e.Status).HasDefaultValue((int)ChatStatus.WAIT_COMMAND);
            base.OnModelCreating(modelBuilder);
        }
    }
}