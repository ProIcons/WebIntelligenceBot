# Discord Bot for Web Intelligence Master
<p align="center">
    <img alt='pmsLogo' src='https://msc.iee.ihu.gr/wp-content/uploads/2019/06/cropped-newlogo-8.png'/>
</p>

**What you'll need**

- Latest .NET 6 SDK
- PostgresSQL
- Discord Bot account

**How to get this bot running in development**

- Set up a bot account on the [Discord developer portal](https://discord.com/developers/applications)
    - Ensure you have the following priviledged gateway intents enabled:
        - Presence Intent
        - Server Members Intent
- Clone/fork the repository from `main` branch
- Get the Id of the Discord Guild you will be testing the bot in, for the purposes of Slash command updating
- Get your bot token from the [Discord developer portal](https://discord.com/developers/applications)
- Set up configurations for development, using [user-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
    - `dotnet user-secrets set DiscordConfiguration:GuildId GUILD_ID`
    - `dotnet user-secrets set DiscordConfiguration:BotToken BOT_TOKEN`

By default the bot will look for a SQL Server instance running on `localhost`. If your instance is not on `localhost` or has an otherwise differing connection string, set the `ConnectionStrings:Database` secret.

**Invite your bot**

(Change your client Id to that of your application's)

```https://discord.com/oauth2/authorize?client_id=CLIENT_ID&scope=bot%20applications.commands&permissions=1573252310```

This ensures the bot has the minimum required permissions and can manage Slash commands on the guild.

Start the bot. This will apply migrations automatically via Entity Framework.

### How to self host

Currently you will need to build from source. There are no distributions at this time.

**Requirements**
- PostgresSQL
- Web host for ASP.NET Core

Set environment variables for `ConnectionStrings:Database`, `DiscordConfiguration:GuildId`, `DiscordConfiguration:BotToken`.

This bot is intended for single-guild usage.

### Credits

This project is based a project i Contribute, [Accord](https://github.com/patrickklaeren/Accord) originally created by [Patrick Klaeren](https://github.com/patrickklaeren/)

Notable dependencies for this project include:
- [Remora](https://github.com/Nihlus/Remora.Discord)
- [MediatR](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
