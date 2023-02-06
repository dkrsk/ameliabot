using System;

using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using DSharpPlus.Entities;

using DnKR.AmeliaBot.BotCommands;
using DnKR.AmeliaBot.Music;

namespace DnKR.AmeliaBot
{
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
        private readonly LavalinkExtension lavalink;
        private readonly LavalinkConfiguration lavaConfig;

        public Bot(string token)
        {
            dconf = new()
            {
                AutoReconnect = true,
                Token = token,
                TokenType = TokenType.Bot
            };
            
            discord = new(dconf);
            Client = discord;
            SlashCommandsConfiguration commandsConfiguration = new();
            
            slashs = discord.UseSlashCommands();
            slashs.RegisterCommands<MainCommands>(820240588556337164UL);
            slashs.RegisterCommands<MusicCommands>(820240588556337164UL);

            discord.ComponentInteractionCreated += Events.ButtonSearchClicked;

            var endpoint = new ConnectionEndpoint("127.0.0.1", 2334);
            lavaConfig = new LavalinkConfiguration()
            {
                Password = "youshallnotpass",
                SocketEndpoint = endpoint,
                RestEndpoint = endpoint
            };
            lavalink = discord.UseLavalink();
            

        }

        ~Bot()
        {
            Lava.node.StopAsync().GetAwaiter().GetResult();
        }

        public async Task Run()
        {
            await this.discord.ConnectAsync();
            await this.lavalink.ConnectAsync(this.lavaConfig);
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

        public static void CreatePlaylist(InteractionContext ctx)
        {
            Playlists.Add(ctx.Guild, new GuildPlaylist(ctx));
        }

        public static async Task RemovePlaylistAsync(DiscordGuild guild)
        {
            var playlist = Playlists[guild];
            await playlist.Connection.StopAsync();
            Playlists.Remove(guild);
        }

        
    }
}
