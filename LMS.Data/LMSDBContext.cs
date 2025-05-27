using LMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data;

public class LMSDBContext : DbContext
{
    public LMSDBContext(DbContextOptions<LMSDBContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Username=postgres;Password=mysecretpassword;Database=task1db",
            options => options.MigrationsAssembly("LMS.Data"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .Property(m => m.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Review>()
            .Property(r => r.Id)
            .ValueGeneratedNever();
        
        // Movie - Review (One-to-Many)
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Reviews)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId);

        // User - Review (One-to-Many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);
    }
}