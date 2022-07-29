using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Hosting.Extensions;

namespace ReactionDiscordBot
{
    public class Program
    {
        public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .AddDiscordService
            (
                services =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();

                    return configuration.GetValue<string?>("REMORA_BOT_TOKEN") ??
                           throw new InvalidOperationException
                           (
                               "No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a " +
                               "valid token."
                           );
                }
            )
            .ConfigureServices
            (
                (_, services) =>
                {
                    services.Configure<DiscordGatewayClientOptions>(g =>
                        {
                            g.Intents |= GatewayIntents.GuildMessageReactions;
                            g.Intents |= GatewayIntents.GuildMembers;
                        });

                    services
                        .AddResponder<ReactionHandler>()
                        .BuildServiceProvider();

                    services.AddHttpClient();
                }
            )
            .ConfigureLogging
            (
                c => c
                    .AddConsole()
                    .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                    .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
            );
    }
}