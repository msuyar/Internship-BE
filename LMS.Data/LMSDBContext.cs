using LMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data;

public class LMSDBContext : DbContext
{
    public LMSDBContext(DbContextOptions<LMSDBContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Username=postgres;Password=mysecretpassword;Database=task1db",
            options => options.MigrationsAssembly("LMS.Data"));
    }
}