using DnKR.AmeliaBot.Music;

using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Options;

using DSharpPlus.Entities;

using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Clients;
using Lavalink4NET.Players.Queued;
using System.Reflection;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;


public partial class MusicCommands
{
    private readonly IAudioService audioService;

    private delegate bool ProviderCheck(string q);
    private readonly ProviderCheck[] providerCheckers;

    private static MusicCommands instance;

    public static MusicCommands GetInstance(IAudioService audioService)
    {
        if (instance == null)
            instance = new MusicCommands(audioService);
        return instance;
    }

    private MusicCommands(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        this.audioService = audioService;

        providerCheckers = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(ProviderCheckerAttribute), false).Length > 0)
                .Select(m => m.CreateDelegate<ProviderCheck>(this))
                .ToArray(); // наверн надо сделать мапу чекер:плей и в основном плейе это все хандлить я хз хочу спать
    }

    private async ValueTask<GuildPlaylist?> GetPlaylistAsync(CommonContext ctx, bool connectToVoiceChannel = true)
    {
        var options = new GuildPlaylistOptions() { Context = ctx, SelfDeaf = true, HistoryCapacity = 10 };
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Move : PlayerChannelBehavior.None,
            VoiceStateBehavior: MemberVoiceStateBehavior.Ignore);
        
        PlayerResult<GuildPlaylist> result; // skipcq: CS-W1022
        try
        {
            result = await audioService.Players
            .RetrieveAsync<GuildPlaylist, GuildPlaylistOptions>(ctx.Guild.Id, ctx.Member.VoiceState?.Channel?.Id, GuildPlaylist.CreatePlayerAsync, Options.Create(options), retrieveOptions)
            .ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.ShortErrorEmbed(ctx.Member)).ConfigureAwait(false);
            return null;
        }

        if (!result.IsSuccess)
        {
            DiscordEmbed embed = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => GlobalEmbeds.UniEmbed("Ты не подключен к голосовому каналу!", ctx.Member),
                _ => GlobalEmbeds.ShortErrorEmbed(ctx.Member)
            };
            await ctx.RespondEmbedAsync(embed).ConfigureAwait(false); // !!!!!!!!
            return null;
        }

        return result.Player;
    }

    public async Task JoinAsync(CommonContext ctx)
    {
        var player = await GetPlaylistAsync(ctx); if (player is null) return;
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Подключилась к `{player.Channel.Name}`", ctx.Member)).ConfigureAwait(false);
    }

    public async Task LeaveAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        await playlist.DisconnectAsync();
        await playlist.DisposeAsync();

        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Пока!", ctx.Member));
    }

    public async Task PlayAsync(CommonContext ctx, string query, bool playTop)
    {
        if (query == "pause")
        {
            await PauseAsync(ctx);
            return;
        }

        await ctx.DeferAsync(false);

        var playlist = await GetPlaylistAsync(ctx);
        if (playlist is null) return;

        var searchResult = await audioService.Tracks
            .LoadTracksAsync(query, TrackSearchMode.YouTube);
        
        if(searchResult.IsFailed)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        if (searchResult.IsPlaylist)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{searchResult.Playlist.Name} добавлен", ctx.Member));
            
            await playlist.PlayAsync(searchResult.Track);
            foreach(var track in searchResult.Tracks[1..])
            {
                await playlist.Queue.AddAsync(new TrackQueueItem(track));
            }
            return;
        }

        await ctx.RespondEmbedAsync(MusicEmbeds.TrackAdded(searchResult.Track, ctx.Member));

        if(playTop)
        {
            await playlist.Queue.InsertAsync(0, new TrackQueueItem(searchResult.Track));
        }
        else
            await playlist.PlayAsync(searchResult.Track);

    }

    public async Task SearchAsync(CommonContext ctx, string query)
    {
        await ctx.DeferAsync(false);

        var playlist = await GetPlaylistAsync(ctx); if (playlist is null) return;

        var searchResult = await audioService.Tracks
            .LoadTracksAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false); ;

        if (!searchResult.IsSuccess)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        var tracks = searchResult.Tracks.ToArray();

        playlist.SearchResults = tracks[..5];

        var searchEmbed = MusicEmbeds.SearchEmbed(tracks, ctx.Member);
        
        await ctx.RespondEmbedAsync(searchEmbed.Item1, false, searchEmbed.Item2);
    }

    public async Task SkipAsync(CommonContext ctx, long count)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if(playlist is null) return;

        if(playlist.CurrentTrack != null)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"`{playlist.CurrentTrack.Title}` пропущен.", ctx.Member));
            await playlist.SkipAsync((int)count);
            return;
        }
        await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    public async Task QueueAsync(CommonContext ctx)
    {
        await ctx.DeferAsync().ConfigureAwait(false);
        var playlist = await audioService.Players.GetPlayerAsync<GuildPlaylist>(ctx.Guild.Id);
        await ctx.RespondEmbedAsync(MusicEmbeds.QueueEmbed(playlist, ctx.Member));
    }

    public async Task RemoveAsync(CommonContext ctx, long position)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        if(playlist.Queue.Count > position - 1)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"`{position}.` {playlist.Queue[(int)position-1].Track.Title} удален.", ctx.Member));
            await playlist.Queue.RemoveAtAsync((int)position - 1).ConfigureAwait(false);
        }
        else
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Невозможно удалить трек `{position}`. Неверный номер.", ctx.Member));
        }
    }

    public async Task LoopAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        if (playlist.CurrentTrack != null)
        {
            playlist.RepeatMode = playlist.RepeatMode == TrackRepeatMode.Track ? TrackRepeatMode.None : TrackRepeatMode.Track;
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Трек {(playlist.RepeatMode == TrackRepeatMode.Track ? "зациклен!" : "расциклен!"):ok_hand:}", ctx.Member));
        }
        else
            await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    public async Task ClearAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        await playlist.StopAsync().ConfigureAwait(false);
        await playlist.Queue.ClearAsync().ConfigureAwait(false);
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Очередь очищена!", ctx.Member));
    }

    public async Task PauseAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx);
        if (playlist is null) return;

        if (playlist.CurrentTrack != null)
        {
            string answer = playlist.IsPaused ? "Продолжаем!:ok_hand:" : "Приостановлено!:ok_hand:";
            await playlist.ControlPauseAsync();
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(answer, ctx.Member));
        }
        else await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    public async Task PlaySkipAsync(CommonContext ctx, string query)
    {
        var playlist = await GetPlaylistAsync(ctx);
        if (playlist is null) return;

        await PlayAsync(ctx, query, true); 
        await SkipAsync(ctx, 1);
    }

    public async Task PlayPreviousAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        if (playlist.CurrentTrack != null)
        {
            if(!playlist.Queue.HasHistory || playlist.Position.Value.Position.TotalSeconds >= 5)
            {
                await playlist.SeekAsync(new TimeSpan(0, 0, 0), SeekOrigin.Begin).ConfigureAwait(false);
                await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Трек перемотан на начало", ctx.Member));
            }
            else
            {
                await playlist.Queue.InsertAsync(0, playlist.Queue.History[0]);
                await SkipAsync(ctx, 1);
            }
        }
        else
            await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    [GeneratedRegex("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$")]
    private static partial Regex YtRegex();
}
