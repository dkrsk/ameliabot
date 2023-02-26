using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

using DnKR.AmeliaBot.Music;
using DSharpPlus.CommandsNext;

namespace DnKR.AmeliaBot.BotCommands;


public class MusicCommands
{
    private static async Task<string> _JoinAsync(CommonContext ctx)
    {
        var lava = Bot.Lava;
        var channel = ctx.Member.VoiceState.Channel;

        if (!lava.lava.ConnectedNodes.Any())
        {
            return "Ошибка lavalink";
        }

        if (channel == null || channel.Type != ChannelType.Voice)
        {
            return "Ты не подключен к голосовому каналу!";
        }

        if (lava.node.GetGuildConnection(ctx.Guild) != null)
        {
            if(lava.node.GetGuildConnection(ctx.Guild).Channel == channel)
            {
                return "Ошибка lavalink";
            }
            await lava.node.GetGuildConnection(ctx.Guild).DisconnectAsync();
        }

        var conn = await lava.node.ConnectAsync(channel);

        Bot.CreatePlaylist(ctx);

        conn.PlaybackFinished += Events.PlaybackFinished;

        return $"Подключилась к {channel.Name}";
    }

    public static async Task JoinAsync(CommonContext ctx)
    {
        await ctx.RespondAsync(Embeds.UniEmbed(await _JoinAsync(ctx), ctx.Member));
    }

    public static async Task LeaveAsync(CommonContext ctx)
    {
        var lava = Bot.Lava;
        var conn = lava.node.GetGuildConnection(ctx.Guild);

        if (conn == null)
        {
            await ctx.RespondAsync(Embeds.UniEmbed("Но я никуда не подключена!", ctx.Member));
        }

        await Bot.RemovePlaylistAsync(ctx.Guild);
        await conn.DisconnectAsync();
            

        await ctx.RespondAsync(Embeds.UniEmbed("Пока!", ctx.Member));
    }

    public static async Task PlayAsync(CommonContext ctx, string query)
    {
        var lava = Bot.Lava;

        var searchResult = await lava.node.Rest.GetTracksAsync(query);

        if(searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondAsync(Embeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        var track = searchResult.Tracks.First();
        await ctx.RespondAsync(Embeds.TrackAdded(track, ctx.Member));

        await _JoinAsync(ctx);

        var playlist = Bot.GetPlaylist(ctx.Guild);

        await playlist.AddAsync(track);
    }

    public static async Task SearchAsync(CommonContext ctx, string query)
    {
        var lava = Bot.Lava;

        if (ctx.DeferAsync != null)
            await ctx.DeferAsync(false);

        var searchResult = await lava.node.Rest.GetTracksAsync(query);

        if (searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        {
            await ctx.RespondAsync(Embeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }

        //await ctx.DeferAsync(true);

        var tracks = searchResult.Tracks.ToArray();

        await _JoinAsync(ctx);
        var playlist = Bot.GetPlaylist(ctx.Guild);
        playlist.SearchResults = tracks[..5];

        var searchEmbed = Embeds.SearchEmbed(tracks, ctx.Member);

        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(searchEmbed.Item1).AddComponents(searchEmbed.Item2));
        else
            await ctx.RespondAsync(searchEmbed.Item1, false, searchEmbed.Item2);
    }

    public static async Task SkipAsync(CommonContext ctx, long count)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null)
        {
            if(!playlist.Any() && playlist.CurrentTrack == null)
            {
                await ctx.RespondAsync(Embeds.UniEmbed("Очередь пуста!", ctx.Member));
                return;
            }
            await ctx.RespondAsync(Embeds.UniEmbed($"{playlist.CurrentTrack.Title} пропущен.", ctx.Member));
            await playlist.PlayNext((int)count);
        }
    }

    public static async Task QueueAsync(CommonContext ctx)
    {
        if(ctx.DeferAsync != null) await ctx.DeferAsync();
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embeds.QueueEmbed(playlist, ctx.Member)));
        else
            await ctx.RespondAsync(Embeds.QueueEmbed(playlist, ctx.Member));
    }

    public static async Task RemoveAsync(CommonContext ctx, long position)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null && playlist.Count < position)
        {
            playlist.Remove((int)position);
            await ctx.RespondAsync(Embeds.UniEmbed($"`{position}.` {playlist.CurrentTrack} удален.", ctx.Member));
        }
        else
        {
            await ctx.RespondAsync(Embeds.UniEmbed($"Невозможно удалить трек `{position}`. Неверный номер.", ctx.Member));
        }
    }

    public static async Task LoopAsync(CommonContext ctx)
    {
        var playlist = Bot.GetPlaylist(ctx.Guild);
        if(playlist != null && playlist.CurrentTrack != null)
        {
            playlist.ChangeRepeat();
            await ctx.RespondAsync(Embeds.UniEmbed($"Трек {(playlist.IsRepeat ? "зациклен" : "расциклен")}", ctx.Member));
        }
    }


}
