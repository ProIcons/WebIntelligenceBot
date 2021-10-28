using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebIntelligence.Domain;

public class WebIntelligenceDesignTimeContextFactory : IDesignTimeDbContextFactory<WebIntelligenceContext>
{
    WebIntelligenceContext IDesignTimeDbContextFactory<WebIntelligenceContext>.CreateDbContext(string[] args)
    {
        // var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
        //
        // var config = new ConfigurationBuilder()
        //     .AddJsonFile("appsettings.json")
        //     .AddJsonFile($"appsettings{environmentName}.json", true)
        //     .AddEnvironmentVariables()
        //     .Build();

        var builder = new DbContextOptionsBuilder<WebIntelligenceContext>();

        // builder.UseNpgsql(config.GetConnectionString("Database"));
        builder.UseNpgsql("");

        return new WebIntelligenceContext(builder.Options);
    }
}