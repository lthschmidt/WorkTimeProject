using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public string connectionString = string.Empty;

    public ApplicationContext()
        : base()
    {
        Database.EnsureCreated();
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options, string connectionString)
        : base(options)
    {
        this.connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(TransactionConfigure);
    }

    public void TransactionConfigure(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Date)
            .HasDefaultValueSql("DATE('now', 'localtime')")
            .HasColumnType("DATE");
        builder.ToTable(u => u.HasCheckConstraint("ValidHours", "Hours > 0 AND Hours < 24"));
    }
}
