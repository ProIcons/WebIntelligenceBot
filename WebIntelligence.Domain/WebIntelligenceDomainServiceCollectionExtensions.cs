using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebIntelligence.Domain;

public static class WebIntelligenceDomainServiceCollectionExtensions
{
    public static IServiceCollection AddWebIntelligenceDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<WebIntelligenceContext>(x =>
                NpgsqlDbContextOptionsBuilderExtensions.UseNpgsql(x,
                    configuration.GetConnectionString("Database")));

        return services;
    }
}