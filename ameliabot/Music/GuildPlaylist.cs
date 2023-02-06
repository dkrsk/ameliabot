using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot.Music
{
    public class GuildPlaylist
    {
        public LavalinkTrack? CurrentTrack => currentTrack;
        //public List<LavalinkTrack> Playlist => playlist;
        public LavalinkGuildConnection Connection => connection;
        public DiscordChannel Channel => channel;
        public LavaEntities Lava => lava;

        public LavalinkTrack?[] SearchResults { get => searchList; set { searchList = value; } }
        public int Count => playlist.Count;
        public bool IsRepeat { get; private set; } = false;
        

        private readonly LavaEntities lava;
        private LavalinkTrack? currentTrack;
        private readonly LavalinkGuildConnection connection;
        private readonly DiscordChannel channel;
        private DiscordMessage? message;
        private readonly DiscordGuild guild;
        private List<LavalinkTrack> playlist;
        private LavalinkTrack?[] searchList = new LavalinkTrack?[5];

        public GuildPlaylist(InteractionContext ctx)
        {
            lava = Bot.Lava;
            this.guild = ctx.Guild;
            this.connection = lava.node.GetGuildConnection(guild);
            this.channel = ctx.Channel;
            playlist = new List<LavalinkTrack>();
        }

        public bool Any()
        {
            return Count != 0;
        }

        public async Task AddAsync(LavalinkTrack track)
        {
            playlist.Add(track);
            if(currentTrack != null || Any())
            {
                await PlayNext();
            }
        }

        public LavalinkTrack? PopNext()
        {
            if (playlist.Any())
            {
                var track = playlist[0];
                playlist.RemoveAt(0);
                return track;
            }
            return null;
        }

        public LavalinkTrack? GetNext()
        {
            if(playlist.Any())
            {
                return playlist[0];
            }
            return null;
        }

        public async Task PlayNext(int skip)
        {
            if (connection.CurrentState.CurrentTrack != null && skip <= 0)
            {
                return;
            }

            if (skip <= 0 && currentTrack != null && IsRepeat)
            {
                await connection.PlayAsync(currentTrack);
                return;
            }

            LavalinkTrack? track;

            do
            {
                track = PopNext();
                skip--;
            } while (skip > 0);

            if (track != null && connection.IsConnected)
            {
                await connection.PlayAsync(track);
                currentTrack = track;

                await SetMessage();

                return;
            }
            currentTrack = null;
            await SetMessage();
            await connection.StopAsync();
        }

        public async Task PlayNext()
        {
            await PlayNext(0);
        }

        private async Task SetMessage()
        {
            if(currentTrack != null)
            {
                if (message != null)
                {
                    await message.DeleteAsync();
                }
                message = await channel.SendMessageAsync(Embeds.NowPlaying(currentTrack, GetNext()));
                return;
            }
            if(message != null)
            {
                await message.DeleteAsync();
                message = null;
            }
        }

        public void Remove(int index)
        {
            try
            {
                playlist.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        public LavalinkTrack At(int index)
        {
            return playlist[index];
        }

        public void ChangeRepeat()
        {
            IsRepeat = IsRepeat ? false : true;
        }
    }
}
