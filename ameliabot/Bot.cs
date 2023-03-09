using System;

using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using DSharpPlus.Entities;

using DnKR.AmeliaBot.BotCommands;
using DnKR.AmeliaBot.BotCommands.MusicCommands;
using DnKR.AmeliaBot.BotCommands.ChatCommands;
using DnKR.AmeliaBot.Music;


namespace DnKR.AmeliaBot;

public struct LavaEntities
{
    public LavalinkExtension lava;
    public LavalinkNodeConnection node;
    public LavaEntities(LavalinkExtension lava, LavalinkNodeConnection node)
    {
        this.lava = lava;
        this.node = node;
    }
}

public class Bot
{
    public DiscordClient discord;
    public static LavaEntities Lava { get; private set; } = new();
    public static Dictionary<DiscordGuild, GuildPlaylist> Playlists { get; private set; } = new();
    public static DiscordClient Client { get; private set; }

    private readonly DiscordConfiguration dconf;
    private readonly SlashCommandsExtension slashs;
    private readonly CommandsNextExtension next;
    private readonly LavalinkExtension lavalink;
    private readonly LavalinkConfiguration lavaConfig;

    public Bot(string token)
    {
        dconf = new()
        {
            AutoReconnect = true,
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        };
            
        discord = new(dconf);
        Client = discord;

        SlashCommandsConfiguration commandsConfiguration = new();

        slashs = discord.UseSlashCommands();
        // #if DEBUG
        //     slashs.RegisterCommands<MainCommands>(820240588556337164UL); // skipcq: CS-R1076
        //     slashs.RegisterCommands<MusicSlashCommands>(820240588556337164UL); // skipcq: CS-R1076
        //     slashs.RegisterCommands<ChatSlashCommands>(820240588556337164UL); // skipcq: CS-R1076
        // #else
            slashs.RegisterCommands<MainCommands>();
            slashs.RegisterCommands<MusicSlashCommands>();
            slashs.RegisterCommands<ChatSlashCommands>();
        // #endif

        CommandsNextConfiguration nextConfiguration = new()
        {
            StringPrefixes = new[] { "d!", "в!" }
        };

        next = discord.UseCommandsNext(nextConfiguration);
        next.RegisterCommands<MusicNextCommands>();
        next.RegisterCommands<ChatNextCommands>();

        discord.ComponentInteractionCreated += MusicEvents.ButtonSearchClicked;

        var endpoint = new ConnectionEndpoint("127.0.0.1", 2334);
        lavaConfig = new LavalinkConfiguration()
        {
            Password = "youshallnotpass",
            SocketEndpoint = endpoint,
            RestEndpoint = endpoint
        };
        lavalink = discord.UseLavalink();

        discord.MessageCreated += ChatEvents.MessageCreated;
        discord.VoiceStateUpdated += MusicEvents.VoiceStateUpdated;
    }

    ~Bot()
    {
        Lava.node.StopAsync().GetAwaiter().GetResult();
    }

    public async Task RunAsync()
    {
        await this.discord.ConnectAsync();

        // do
        // {
        //     try
        //     {
                await this.lavalink.ConnectAsync(this.lavaConfig);
        //     }
        //     catch { Thread.Sleep(2); }
        // }
        // while (this.lavalink.ConnectedNodes.Count == 0);
        
        Lava = new(lavalink, lavalink.ConnectedNodes.Values.First());
    }

    public static GuildPlaylist? GetPlaylist(DiscordGuild guild)
    {
        if (Playlists.TryGetValue(guild, out GuildPlaylist? playlist))
        {
            return playlist;
        }
        return null;
    }

    public static void CreatePlaylist(CommonContext ctx)
    {
        try
        {
            Playlists.Add(ctx.Guild, new GuildPlaylist(ctx));
        }
        catch (ArgumentException) { }
    }

    public static async Task RemovePlaylistAsync(DiscordGuild guild)
    {
        if (Playlists.TryGetValue(guild, out GuildPlaylist? playlist))
        {
            Playlists.Remove(guild);
        }
        else return;        
    }
        
}
