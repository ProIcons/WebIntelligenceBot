using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using WebIntelligence.Bot.Configuration;

namespace WebIntelligence.Bot;

public class BotHostedService : BackgroundService
{
    private readonly ILogger<BotClient> _logger;
    private readonly DiscordGatewayClient _discordGatewayClient;
    private readonly SlashService _slashService;
    private readonly DiscordConfiguration _discordConfiguration;

    public BotHostedService(SlashService slashService, DiscordGatewayClient discordGatewayClient, ILogger<BotClient> logger, DiscordConfiguration discordConfiguration)
    {
        _slashService = slashService;
        _discordGatewayClient = discordGatewayClient;
        _logger = logger;
        _discordConfiguration = discordConfiguration;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await InitialiseSlashCommands(cancellationToken);

        var runResult = await _discordGatewayClient.RunAsync(cancellationToken);

        if (!runResult.IsSuccess)
        {
            switch (runResult.Error)
            {
                case ExceptionError exe:
                {
                    _logger.LogError(exe.Exception, "Exception during gateway connection: {ExceptionMessage}",
                        exe.Message);
                    break;
                }
                case GatewayWebSocketError:
                case GatewayDiscordError:
                {
                    _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                    break;
                }
                default:
                {
                    _logger.LogError("Unknown error: {Message}", runResult.Error.Message);
                    break;
                }
            }
        }
    }

    private async Task InitialiseSlashCommands(CancellationToken cancellationToken)
    {
        var slashSupport = _slashService.SupportsSlashCommands();

        if (!slashSupport.IsSuccess)
        {
            _logger.LogWarning("The registered commands of the bot don't support slash commands: {Reason}",
                slashSupport.Error.Message);
        }
        else
        {
            var updateSlash = await _slashService.UpdateSlashCommandsAsync(new Snowflake(_discordConfiguration.GuildId),
                ct: cancellationToken);

            if (!updateSlash.IsSuccess)
            {
                _logger.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
            }
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}