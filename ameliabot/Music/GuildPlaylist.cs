using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace DnKR.AmeliaBot.Music;

public class GuildPlaylist
{
    public LavalinkTrack? CurrentTrack => currentTrack;
    public LavalinkGuildConnection Connection => connection;
    public DiscordChannel Channel => channel;

    public LavalinkTrack?[] SearchResults { get => searchList; set => searchList = value; } 
    public int Count => playlist.Count;
    public bool IsRepeat { get; private set; } = false;
        

    private readonly LavaEntities lava;
    private readonly LavalinkGuildConnection connection;
    private readonly DiscordChannel channel;
    private readonly DiscordGuild guild;
    private readonly List<LavalinkTrack> playlist;
    private LavalinkTrack? currentTrack;
    private DiscordMessage? message;
    private LavalinkTrack?[] searchList = new LavalinkTrack?[5];

    public GuildPlaylist(CommonContext ctx)
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
            await PlayNextAsync();
        }
    }

    public async Task AddTopAsync(LavalinkTrack track)
    {
        playlist.Prepend(track);
        if(currentTrack != null || Any())
        {
            await PlayNextAsync();
        }
    }

    public void AddMany(LavalinkTrack[] tracks)
    {
        playlist.AddRange(tracks);
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
            return playlist.First();
        }
        return null;
    }

    public async Task PlayNextAsync(int skip)
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
        IsRepeat = false;

        do
        {
            track = PopNext();
            skip--;
        } while (skip > 0);

        if (track != null && connection.IsConnected)
        {
            await connection.PlayAsync(track);
            currentTrack = track;

            await SetMessageAsync();

            return;
        }
        currentTrack = null;
        await SetMessageAsync();
        await connection.StopAsync();
    }

    public async Task PlayNextAsync()
    {
        await PlayNextAsync(0);
    }

    private async Task SetMessageAsync()
    {
        if(currentTrack != null)
        {
            if (message != null)
            {
                await message.DeleteAsync();
            }
            message = await channel.SendMessageAsync(MusicEmbeds.NowPlaying(currentTrack, GetNext()));
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

    public async Task ClearAsync()
    {
        playlist.Clear();
        await connection.StopAsync();
    }

    public LavalinkTrack At(int index)
    {
        if(index > Count-1)
            throw new ArgumentOutOfRangeException(nameof(index));
        return playlist[index];
    }
    public LavalinkTrack this[int index] => playlist[index];

    public void ChangeRepeat()
    {
        IsRepeat = !IsRepeat;
    }
}
