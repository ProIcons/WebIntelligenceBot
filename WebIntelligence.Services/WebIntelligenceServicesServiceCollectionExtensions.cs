using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Services.HostedServices;
using WebIntelligence.Services.Mapping;

namespace WebIntelligence.Services;

public static class WebIntelligenceServicesServiceCollectionExtensions
{
    public static IServiceCollection AddWebIntelligenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
                .AddSingleton<MappingProfile>()
                .AddAutoMapper((builder) => builder.AddProfile(new MappingProfile()), typeof(Results).Assembly, typeof(WebIntelligenceContext).Assembly)
                .AddHostedService<RemindersHostedService>()
            ;
    }
}