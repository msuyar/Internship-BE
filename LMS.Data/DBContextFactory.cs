using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LMS.Data;

public class DbContextFactory : IDesignTimeDbContextFactory<LMSDBContext>
{
    public LMSDBContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Task1");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json")
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<LMSDBContext>();
        var connectionString = configuration.GetConnectionString("Default");
        optionsBuilder.UseNpgsql(connectionString);
        return new LMSDBContext(optionsBuilder.Options);
    }
}