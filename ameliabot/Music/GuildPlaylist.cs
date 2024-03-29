using DSharpPlus;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DnKR.AmeliaBot.Music;

public sealed record class GuildPlaylistOptions : QueuedLavalinkPlayerOptions
{
    public CommonContext Context { get; init; }
}

public class GuildPlaylist : QueuedLavalinkPlayer
{
    public DiscordChannel Channel => channel;

    public LavalinkTrack?[] SearchResults { get => searchList; set => searchList = value; }

    public ITrackQueueItem? PreviousTrack { get; private set; }

    private readonly DiscordChannel channel;
    private DiscordMessage? message;
    private LavalinkTrack?[] searchList = new LavalinkTrack?[5];

    public GuildPlaylist(IPlayerProperties<GuildPlaylist, GuildPlaylistOptions> properties)
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


    // add enqueued event handle

    protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
    {
        await base
            .NotifyTrackStartedAsync(track, cancellationToken)
            .ConfigureAwait(false);
        
        if (message != null)
        {
            await message.DeleteAsync();
        }
        message = await channel.SendMessageAsync(MusicEmbeds.NowPlaying(CurrentTrack, (LavalinkTrack?)Queue.Peek()));
    }

    protected override async ValueTask NotifyTrackEndedAsync(ITrackQueueItem track, TrackEndReason endReason, CancellationToken cancellationToken)
    {
        await base
            .NotifyTrackEndedAsync(track, endReason, cancellationToken)
            .ConfigureAwait(false);

        if (!Queue.Any())
        {
            await message.DeleteAsync();
            message = null;
        }
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
