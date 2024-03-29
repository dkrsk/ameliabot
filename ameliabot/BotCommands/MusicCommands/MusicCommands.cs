﻿using DSharpPlus;
using Lavalink4NET;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;

using DnKR.AmeliaBot.Music;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public struct JoinMessage
{
    public int Code => code;
    public string Content => content;
    private int code;
    private string content;
    public JoinMessage(int code, string content)
    {
        this.code = code;
        this.content = content;
    }
}

public partial class MusicCommands
{
    private readonly IAudioService audioService;
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
    }

    private async ValueTask<GuildPlaylist> GetPlaylistAsync(CommonContext ctx, bool connectToVoiceChannel = true)
    {
        var options = new GuildPlaylistOptions() { Context = ctx };
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        PlayerResult<GuildPlaylist> result = await audioService.Players
        .RetrieveAsync<GuildPlaylist, GuildPlaylistOptions>(ctx.Guild.Id, ctx.Member?.VoiceState.Channel.Id, GuildPlaylist.CreatePlayerAsync, Options.Create(options), retrieveOptions)
        .ConfigureAwait(false);

        var msg = string.Empty;
        if (!result.IsSuccess)
        {
            msg = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "Ты не подключен к голосовому каналу!",
                PlayerRetrieveStatus.BotNotConnected => "Ошибка lavalink",
                _ => "Ой, что-то сломалось >.<"
            };
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(msg, ctx.Member)).ConfigureAwait(false);
        }
        
        //if(connectToVoiceChannel)
        //{
        //    if (result.Status == PlayerRetrieveStatus.UserInSameVoiceChannel) // idk why this don't work
        //        msg = "Я уже сюда подключена)";
        //    else msg = $"Подключилась к {ctx.Member.VoiceState.Channel.Name}";
        //}

        //if (connectToVoiceChannel)
        //    await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(msg, ctx.Member));
        return result.Player;
    }

    public async Task JoinAsync(CommonContext ctx)
    {
        await GetPlaylistAsync(ctx);
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

        var playlist = await GetPlaylistAsync(ctx);
        if (playlist is null) return;

        // TODO: add playlist search feature
        Regex ytRegex = YtRegex();
        var searchMode = ytRegex.IsMatch(query) ?
            TrackSearchMode.None
            : TrackSearchMode.YouTube;
        var searchResult = await audioService.Tracks
            .LoadTrackAsync(query, searchMode)
            .ConfigureAwait(false);

        if(searchResult is null)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        if(playTop)
        {
            await playlist.Queue.InsertAsync(0, (ITrackQueueItem)searchResult); // Maybe it doesn't work
        }
        else
            await playlist.PlayAsync(searchResult).ConfigureAwait(false);
        await ctx.RespondEmbedAsync(MusicEmbeds.TrackAdded(searchResult, ctx.Member)).ConfigureAwait(false);
    }

    public async Task SearchAsync(CommonContext ctx, string query)
    {
        if (ctx.DeferAsync != null)
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

        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(searchEmbed.Item1).AddComponents(searchEmbed.Item2));
        else
            await ctx.RespondEmbedAsync(searchEmbed.Item1, false, searchEmbed.Item2);
    }

    public async Task SkipAsync(CommonContext ctx, long count)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if(playlist.CurrentTrack != null)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{playlist.CurrentTrack.Title} пропущен.", ctx.Member));
            await playlist.SkipAsync().ConfigureAwait(false);
            return;
        }
        await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    public async Task QueueAsync(CommonContext ctx)
    {
        if(ctx.DeferAsync != null) await ctx.DeferAsync().ConfigureAwait(false);
        var playlist = await GetPlaylistAsync(ctx, false);
        var emb = MusicEmbeds.QueueEmbed(playlist, ctx.Member);
    
        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb));
        else
            await ctx.RespondEmbedAsync(emb);
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
            playlist.RepeatMode = Lavalink4NET.Players.Queued.TrackRepeatMode.Track;
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Трек {(playlist.RepeatMode == Lavalink4NET.Players.Queued.TrackRepeatMode.Track ? "зациклен" : "расциклен")}", ctx.Member));
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
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        if (playlist.CurrentTrack != null)
        {
            string answer = playlist.IsPaused ? "Продолжаем!:ok_hand:" : "Приостоновленно!:ok_hand:";
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
        await playlist.SkipAsync().ConfigureAwait(false);
    }

    public async Task PlayPreviousAsync(CommonContext ctx)
    {
        var playlist = await GetPlaylistAsync(ctx, false);
        if (playlist is null) return;

        if (playlist.CurrentTrack != null)
        {
            if(playlist.Queue.HasHistory || playlist.Position.Value.Position.TotalSeconds >= 5)
            {
                await playlist.SeekAsync(new TimeSpan(0, 0, 0), SeekOrigin.Begin).ConfigureAwait(false);
                await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Трек перемотан на начало", ctx.Member));
            }
            else
            {
                await playlist.Queue.InsertAsync(0, playlist.Queue.History[0]);
                await playlist.SkipAsync().ConfigureAwait(false);
            }
        }
        else
            await ctx.RespondEmbedAsync(MusicEmbeds.EmptyQueueEmbed(ctx.Member));
    }

    [GeneratedRegex("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$")]
    private static partial Regex YtRegex();
}
