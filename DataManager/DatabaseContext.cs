using DataManager.Mapping;
using DataManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManager;

/// <summary>
/// Represents the database context for managing data entities.
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Gets or sets the DbSet for the Chat entity.
    /// </summary>
    public DbSet<Chat> Chats { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for the ChatFile entity.
    /// </summary>
    public DbSet<ChatFile> Files { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for the Selection entity.
    /// </summary>
    public DbSet<Selection> Selections { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for the SelectionParams entity.
    /// </summary>
    public DbSet<SelectionParams> SelectionParams { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for the Sorting entity.
    /// </summary>
    public DbSet<Sorting> Sortings { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for the SortingParams entity.
    /// </summary>
    public DbSet<SortingParams> SortingParams { get; set; }


    /// <summary>
    /// Default db context constructor.
    /// </summary>
    public DatabaseContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(@"Server=db;Username=postgres;Password=postgres;Database=postgres");

    /// <summary>
    /// Configures model behavior during database creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder used for configuring the database model.</param>
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