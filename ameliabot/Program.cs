using DnKR.AmeliaBot.BotCommands.MusicCommands;
using DSharpPlus;

using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Runtime.InteropServices;

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

        Console.WriteLine(
            "                          _ _       \r\n" +
            "     /\\                  | (_)      \r\n" +
            "    /  \\   _ __ ___   ___| |_  __ _ \r\n" +
            "   / /\\ \\ | '_ ` _ \\ / _ \\ | |/ _` |\r\n" +
            "  / ____ \\| | | | | |  __/ | | (_| |\r\n" +
            " /_/    \\_\\_| |_| |_|\\___|_|_|\\__,_|\r\n" +
            "                                    \r\n" +
            "                                    "
        );
        try
        {
            using (var fs = new StreamReader("version")) // skipcq: CS-R1050
            {
                Console.WriteLine("\tCommit: " + fs.ReadLine()?[..6]); // commit
                Console.WriteLine("\tCommit time: " + fs.ReadLine()); // commit time
                Console.WriteLine("\tBranch: " + fs.ReadLine()); // commit branch
                Console.WriteLine("\tCopyright: Damian Korsakov");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("WARN: version file is broken. " + e.Message);
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
        });

        builder.Services.AddSingleton<MusicCommands>();

        builder.Services.AddLogging(s => s.AddConsole()
            #if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
            #else
            .SetMinimumLevel(LogLevel.Information)
            #endif
        );
        builder.Build().Run();
    }
}
