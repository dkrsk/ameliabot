using DnKR.AmeliaBot.Music;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

partial class MusicCommands
{
    [ProviderLoader]
    private async ValueTask<(LavalinkTrack?, bool)> LoadYtAsync(CommonContext ctx, GuildPlaylist playlist, string query)
    {
        if (!(query.StartsWith("ytsearch:") || YtRegex().Match(query).Success)) return (null, false);

        var searchResult = await audioService.Tracks
            .LoadTracksAsync(query.Substring(9), TrackSearchMode.YouTube);

        if (searchResult.IsFailed) return (null, false);

        if (searchResult.IsPlaylist)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{searchResult.Playlist.Name} добавлен", ctx.Member));

            await playlist.PlayAsync(searchResult.Track);
            foreach (var track in searchResult.Tracks[1..])
            {
                await playlist.Queue.AddAsync(new TrackQueueItem(track));
            }
            return (null, true);
        }

        return (searchResult.Tracks[0], true);

    }
}
