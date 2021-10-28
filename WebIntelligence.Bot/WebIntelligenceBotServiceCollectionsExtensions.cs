using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using WebIntelligence.Bot.CommandGroups;
using WebIntelligence.Bot.Commands;
using WebIntelligence.Bot.Configuration;
using WebIntelligence.Bot.Helpers;
using WebIntelligence.Bot.Helpers.Permissions;
using WebIntelligence.Bot.Parsers;
using WebIntelligence.Bot.Services;
using CommandResponder = Remora.Discord.Commands.Responders.CommandResponder;
using TimeSpanParser = Remora.Commands.Parsers.TimeSpanParser;

namespace WebIntelligence.Bot;

public static class WebIntelligenceBotServiceCollectionsExtensions
{
    public static IServiceCollection AddWebIntelligenceBot(this IServiceCollection services, IConfiguration configuration)
    {
        var discordConfigurationSection = configuration.GetSection("DiscordConfiguration");

        var token = discordConfigurationSection["BotToken"];

        services
            .Configure<DiscordConfiguration>(discordConfigurationSection);
        services
            .AddLogging()
            .AddTransient<BotClient>()
            .AddSingleton<BotState>()
            .AddSingleton<DiscordCache>()
            .AddScoped<DiscordAvatarHelper>()
            .AddScoped<UserFeedbackService>()
            .AddScoped<DiscordPermissionHelper>()
            .AddScoped<DiscordScopedCache>()
            .AddScoped<DiscordChannelParser>()
            .AddScoped<DiscordCache>()
            .AddScoped<CommandResponder>()
            .AddDiscordGateway(_ => token)
            .Configure<DiscordGatewayClientOptions>(o =>
            {
                o.Intents |= GatewayIntents.GuildPresences;
                o.Intents |= GatewayIntents.GuildVoiceStates;
                o.Intents |= GatewayIntents.GuildMembers;
                o.Intents |= GatewayIntents.GuildMessages;
            })
            .AddDiscordCommands(true)
            .AddExecutionEvent<CommandExecutionEventRespondHandler>()
            .AddParser<TimeSpanParser>()
            .AddCommandGroup<ReminderCommandGroup>();


        var responderTypes = typeof(BotClient).Assembly
            .GetExportedTypes()
            .Where(t => t.IsResponder());

        foreach (var responderType in responderTypes)
        {
            services.AddResponder(responderType);
        }
        
        services
            .AddHostedService<BotHostedService>();

        return services;
    }
}