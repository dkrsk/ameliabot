using DnKR.AmeliaBot.BotCommands.MusicCommands;
using DSharpPlus;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DnKR.AmeliaBot;

internal class Program
{
    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Write the bot token");
            return;
        }

        var builder = new HostApplicationBuilder();
        builder.Services.AddHostedService<Bot>();
        builder.Services.AddSingleton<DiscordClient>();
        builder.Services.AddSingleton(new DiscordConfiguration
        {
            AutoReconnect = true,
            Token = args[0],
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds | DiscordIntents.GuildMessages | DiscordIntents.GuildVoiceStates | DiscordIntents.MessageContents
        });
        builder.Services.AddLavalink();
        builder.Services.ConfigureLavalink(options =>
        {
            options.Passphrase = "youshallnotpass";
            options.BaseAddress = new Uri("http://127.0.0.1:2334/");
            options.WebSocketUri = new Uri("ws://127.0.0.1:2334/v4/websocket");
            options.ReadyTimeout = TimeSpan.FromSeconds(10);
            options.HttpClientName = "LavalinkHttpClient/1.0";
        });

        builder.Services.AddSingleton<MusicCommands>();

        builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));
        builder.Build().Run();
    }
}
