using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sentry;
using Serilog;
using Serilog.Events;
using WebIntelligence.Bot;
using WebIntelligence.Domain;
using WebIntelligence.Services;

Log.Information("Starting up...");

var builder = WebApplication
    .CreateBuilder(args);

builder.Logging.AddSerilog(CreateLogger());

var servicesBuilder = builder.Services;
servicesBuilder.AddControllers();
servicesBuilder.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebIntelligence", Version = "v1" }); });

servicesBuilder.AddLazyCache();
servicesBuilder.AddMediatR(
    typeof(WebIntelligenceServicesServiceCollectionExtensions).Assembly,
    typeof(WebIntelligenceBotServiceCollectionsExtensions).Assembly);

servicesBuilder.AddWebIntelligenceDomain(builder.Configuration);
servicesBuilder.AddWebIntelligenceBot(builder.Configuration);
servicesBuilder.AddWebIntelligenceServices(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebIntelligence v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.MapGet("/", () => "Hello. There's nothing to see here yet... :)");
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

Log.Information("Host built...");

using (var scope = app.Services.CreateScope())
{
    Log.Information("Scope created...");

    var services = scope.ServiceProvider;

    await using var db = services.GetRequiredService<WebIntelligenceContext>();

    Log.Information("Got Db context");

    await db.Database.MigrateAsync();

    Log.Information("Migrated!");
}

await app.RunAsync();

ILogger CreateLogger()
{
    var loggerConfiguration = new LoggerConfiguration();

    loggerConfiguration.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
        .Enrich.FromLogContext();

    if (builder.Environment.IsDevelopment())
    {
        loggerConfiguration.WriteTo.Console();
    }

    var sentrySection = builder.Configuration.GetSection("SentryConfiguration");

    if (!string.IsNullOrWhiteSpace(sentrySection["Dsn"]))
    {
        loggerConfiguration.WriteTo.Sentry(o =>
        {
            o.MinimumBreadcrumbLevel = LogEventLevel.Information;
            o.MinimumEventLevel = LogEventLevel.Warning;
            o.Dsn = sentrySection["Dsn"];
            o.Environment = sentrySection["Environment"];
            o.BeforeSend = BeforeSend;
        });
    }

    return loggerConfiguration.CreateLogger();
}

SentryEvent? BeforeSend(SentryEvent arg)
{
    var ignoredLogMessages = new[]
    {
        "No matching command could be found.",
        "Guild User requesting the command does not have the required Administrator permission",
        "Unknown interaction"
    };

    if (arg.Message?.Formatted is not null
        && ignoredLogMessages.Any(x => arg.Message.Formatted.Contains(x)))
    {
        return null;
    }

    var hasCode = arg.Extra.TryGetValue("StatusCode", out var code);

    // Don't log 404's
    if (hasCode && code?.ToString() == "404")
        return null;

    return arg;
}