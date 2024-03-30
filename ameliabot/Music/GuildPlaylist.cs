using DSharpPlus.Entities;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace DnKR.AmeliaBot.Music;

public sealed record class GuildPlaylistOptions : QueuedLavalinkPlayerOptions
{
    public CommonContext Context { get; init; }
}

public class GuildPlaylist : QueuedLavalinkPlayer
{
    public DiscordChannel Channel => channel;

    public LavalinkTrack?[] SearchResults { set => searchList = value; }

    private readonly DiscordChannel channel;
    private DiscordMessage? message;
    private LavalinkTrack?[] searchList = new LavalinkTrack?[5];

    private GuildPlaylist(IPlayerProperties<GuildPlaylist, GuildPlaylistOptions> properties)
        : base(properties)
    {
        this.channel = properties.Options.Value.Context.Channel;
    }

    public static ValueTask<GuildPlaylist> CreatePlayerAsync(IPlayerProperties<GuildPlaylist, GuildPlaylistOptions> properties, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        return ValueTask.FromResult(new GuildPlaylist(properties));
    }

    public LavalinkTrack?[] GetSearchResults() => (LavalinkTrack?[]) searchList.Clone();


    // add enqueued event handle

    protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
    {
        await base
            .NotifyTrackStartedAsync(track, cancellationToken)
            .ConfigureAwait(false);
        
        if (message is not null)
        {
            await message.DeleteAsync();
        }
        message = await channel.SendMessageAsync(MusicEmbeds.NowPlaying((LavalinkTrack)track));
    }

    protected override async ValueTask NotifyTrackEndedAsync(ITrackQueueItem track, TrackEndReason endReason, CancellationToken cancellationToken = default)
    {
        await base
            .NotifyTrackEndedAsync(track, endReason, cancellationToken)
            .ConfigureAwait(false);

        if (!Queue.Any())
        {
            if(message is not null) await message.DeleteAsync();
            message = null;
        }
    }

    protected override async ValueTask NotifyTrackExceptionAsync(ITrackQueueItem track, TrackException exception, CancellationToken cancellationToken = default)
    {
        await base
            .NotifyTrackExceptionAsync(track, exception, cancellationToken)
            .ConfigureAwait(false);

        await channel.SendMessageAsync(GlobalEmbeds.DetailedEmbed(new GlobalEmbeds.DetailedEmbedContent() {
            Description = $"Произошла ошибка при проигрывании `{track.Track?.Title}`! >.<",
            Footer = exception.Message
        }));
    }

    public async Task ControlPauseAsync()
    {
        if (IsPaused)
        {
            await ResumeAsync();
            return;
        }

        await PauseAsync();
        await SeekAsync(new TimeSpan(0, 0, -3), SeekOrigin.Current).ConfigureAwait(false);
    }
}
