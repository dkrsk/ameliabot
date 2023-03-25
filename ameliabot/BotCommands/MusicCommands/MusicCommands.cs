﻿using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;

using DnKR.AmeliaBot.Music;
using System;

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

public static class MusicCommands
{
    private static async Task<JoinMessage> TryJoinAsync(CommonContext ctx)
    {
        var lava = Bot.Lava;
        DiscordChannel? channel = ctx.Member.VoiceState != null ? ctx.Member.VoiceState.Channel : null;

        if (!lava.lava.ConnectedNodes.Any())
        {
            return new(1,"Ошибка lavalink");
        }

        if (channel == null || channel.Type != ChannelType.Voice)
        {
            return new(2,"Ты не подключен к голосовому каналу!");
        }

        var guildConnection = lava.node.GetGuildConnection(ctx.Guild);
        if (guildConnection != null)
        {
            if(guildConnection.Channel != channel)
            {
                await guildConnection.DisconnectAsync(false);
            }
            else
            {
                return new(0, "Я уже сюда подключена)");
            }
        }

        var conn = await lava.node.ConnectAsync(channel);

        Bot.CreatePlaylist(ctx);

        conn.PlaybackFinished += MusicEvents.PlaybackFinished;

        return new(0,$"Подключилась к {channel.Name}");
    }

    public static async Task JoinAsync(CommonContext ctx)
    {
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed((await TryJoinAsync(ctx)).Content, ctx.Member));
    }

    public static async Task LeaveAsync(CommonContext ctx)
    {
        var lava = Bot.Lava;
        var conn = lava.node.GetGuildConnection(ctx.Guild);


        if (conn == null)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Но я никуда не подключена!", ctx.Member));
            return;
        }

        await Bot.RemovePlaylistAsync(ctx.Guild);

        await conn.StopAsync();
        await conn.DisconnectAsync();

        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Пока!", ctx.Member));
    }

    public static async Task PlayAsync(CommonContext ctx, string query, bool playTop)
    {
        var lava = Bot.Lava;

        Regex ytRegex = new(@"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
        LavalinkLoadResult searchResult;

        if (ytRegex.IsMatch(query))
        {
            searchResult = await lava.node.Rest.GetTracksAsync(query, LavalinkSearchType.Plain);
        }
        else searchResult = await lava.node.Rest.GetTracksAsync(query);

        var joinMessage = await TryJoinAsync(ctx);
        if(joinMessage.Code != 0)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(joinMessage.Content, ctx.Member));
            return;
        }

        if(searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        var track = searchResult.Tracks.First();
        await ctx.RespondEmbedAsync(MusicEmbeds.TrackAdded(track, ctx.Member));

        var playlist = Bot.GetPlaylist(ctx.Guild);
        if (playlist != null)
        {
            if (playTop)
                await playlist.AddTopAsync(track);
            else
                await playlist.AddAsync(track);

            if (query.Contains("playlist?list"))
                playlist.AddMany(searchResult.Tracks.ToArray()[1..]);
        }
        else throw new NullReferenceException($"{nameof(TryJoinAsync)} wasn't create playlist instance for some reason");
    }

    public static async Task SearchAsync(CommonContext ctx, string query)
    {
        var lava = Bot.Lava;

        if (ctx.DeferAsync != null)
            await ctx.DeferAsync(false);

        var searchResult = await lava.node.Rest.GetTracksAsync(query);

        var joinMessage = await TryJoinAsync(ctx);
        if (joinMessage.Code != 0)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(joinMessage.Content, ctx.Member));
            return;
        }

        if (searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        var tracks = searchResult.Tracks.ToArray();

        await TryJoinAsync(ctx);
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if (playlist != null)
            playlist.SearchResults = tracks[..5];
        else throw new ArgumentNullException($"{nameof(TryJoinAsync)} wasn't create playlist instance for some reason");

        var searchEmbed = MusicEmbeds.SearchEmbed(tracks, ctx.Member);

        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(searchEmbed.Item1).AddComponents(searchEmbed.Item2));
        else
            await ctx.RespondEmbedAsync(searchEmbed.Item1, false, searchEmbed.Item2);
    }

    public static async Task SkipAsync(CommonContext ctx, long count)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null && playlist.CurrentTrack != null)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{playlist.CurrentTrack.Title} пропущен.", ctx.Member));
            await playlist.PlayNextAsync((int)count);
            return;
        }
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Ничего не воспроизводится!", ctx.Member));
    }

    public static async Task QueueAsync(CommonContext ctx)
    {
        if(ctx.DeferAsync != null) await ctx.DeferAsync();
        var playlist = Bot.GetPlaylist(ctx.Guild);
        var emb = MusicEmbeds.QueueEmbed(playlist, ctx.Member);
    
        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(emb));
        else
            await ctx.RespondEmbedAsync(emb);
    }

    public static async Task RemoveAsync(CommonContext ctx, long position)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null && playlist.Count > position - 1)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"`{position}.` {playlist[(int)position-1].Title} удален.", ctx.Member));
            playlist.Remove((int)position - 1);
        }
        else
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Невозможно удалить трек `{position}`. Неверный номер.", ctx.Member));
        }
    }

    public static async Task LoopAsync(CommonContext ctx)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if (playlist != null)
        {
            if (playlist.CurrentTrack != null)
            {
                playlist.ChangeRepeat();
                await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Трек {(playlist.IsRepeat ? "зациклен" : "расциклен")}", ctx.Member));
            }
            else
                await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Но очередь пуста!", ctx.Member));
        }
        else
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Но я никуда не подключена!", ctx.Member));
    }

    public static async Task ClearAsync(CommonContext ctx)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null)
        {
            await playlist.ClearAsync();
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Очередь очищена!", ctx.Member));
        }
        else
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Но я никуда не подключена!", ctx.Member));
    }

}
