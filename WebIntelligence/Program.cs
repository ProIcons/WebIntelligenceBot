using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;
using Serilog;
using Serilog.Events;
using WebIntelligence.Domain;

namespace WebIntelligence
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Information("Starting up...");

            var host = CreateHostBuilder(args).Build();

            Log.Information("Host built...");

            using (var scope = host.Services.CreateScope())
            {
                Log.Information("Scope created...");

                var services = scope.ServiceProvider;

                await using var db = services.GetRequiredService<WebIntelligenceContext>();

                Log.Information("Got Db context");

                await db.Database.MigrateAsync();

                Log.Information("Migrated!");
            }

            Log.Information("Ready to run");

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog((context, logConfiguration) =>
                    {
                        logConfiguration.MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                            .Enrich.FromLogContext();

                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            logConfiguration.WriteTo.Console();
                        }

                        var sentrySection = context.Configuration.GetSection("SentryConfiguration");

                        if (!string.IsNullOrWhiteSpace(sentrySection["Dsn"]))
                        {
                            logConfiguration.WriteTo.Sentry(o =>
                            {
                                o.MinimumBreadcrumbLevel = LogEventLevel.Information;
                                o.MinimumEventLevel = LogEventLevel.Warning;
                                o.Dsn = sentrySection["Dsn"];
                                o.Environment = sentrySection["Environment"];
                                o.BeforeSend = BeforeSend;
                            });
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                });

        private static readonly string[] IgnoredLogMessages = new[]
        {
            "No matching command could be found.",
            "Guild User requesting the command does not have the required Administrator permission",
            "Unknown interaction"
        };

        private static SentryEvent? BeforeSend(SentryEvent arg)
        {
            if (arg.Message?.Formatted is not null
                && IgnoredLogMessages.Any(x => arg.Message.Formatted.Contains(x)))
            {
                return null;
            }

            var hasCode = arg.Extra.TryGetValue("StatusCode", out var code);

            // Don't log 404's
            if (hasCode && code?.ToString() == "404")
                return null;

            return arg;
        }
    }
}