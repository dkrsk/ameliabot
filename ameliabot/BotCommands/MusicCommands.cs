using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

using DnKR.AmeliaBot.Music;

namespace DnKR.AmeliaBot.BotCommands
{
    public class MusicCommands : ApplicationCommandModule
    {
        public async Task<string> JoinAsync(InteractionContext ctx)
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

        [SlashCommand("join", "Подключиться к твоему голосовому каналу")]
        public async Task JoinCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(Embeds.UniEmbed(await JoinAsync(ctx), ctx.Member));
        }

        [SlashCommand("leave", "Покинуть голосовой канал")]
        public async Task LeaveAsync(InteractionContext ctx)
        {
            var lava = Bot.Lava;
            var conn = lava.node.GetGuildConnection(ctx.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"Но я никуда не подключена!", ctx.Member));
                return;
            }

            await Bot.RemovePlaylistAsync(ctx.Guild);
            await conn.DisconnectAsync();
            

            await ctx.CreateResponseAsync(Embeds.UniEmbed($"Пока!", ctx.Member));
        }

        [SlashCommand("play", "Добавить трек в очередь")]
        public async Task Play(InteractionContext ctx, [Option("название", "Название трека")] string query)
        {
            var lava = Bot.Lava;

            var searchResult = await lava.node.Rest.GetTracksAsync(query);

            if(searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
                return;
            }

            var track = searchResult.Tracks.First();
            await ctx.CreateResponseAsync(Embeds.TrackAdded(track, ctx.Member));

            await this.JoinAsync(ctx);

            var playlist = Bot.GetPlaylist(ctx.Guild);

            await playlist.AddAsync(track);
        }

        [SlashCommand("search", "Выбрать трек из первых 10 по поиску")]
        public async Task Search(InteractionContext ctx, [Option("название", "Название трека")] string query)
        {
            var lava = Bot.Lava;

            await ctx.DeferAsync(false);

            var searchResult = await lava.node.Rest.GetTracksAsync(query);

            if (searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed || searchResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
                return;
            }

            //await ctx.DeferAsync(true);

            var tracks = searchResult.Tracks.ToArray();

            await this.JoinAsync(ctx);
            var playlist = Bot.GetPlaylist(ctx.Guild);
            playlist.SearchResults = tracks[..5];

            //await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, Embeds.SearchEmbed(tracks, ctx.Member));
            await ctx.EditResponseAsync(Embeds.SearchEmbed(tracks, ctx.Member));

        }

        [SlashCommand("skip", "Пропустить текущий трек")]
        public async Task Skip(InteractionContext ctx, [Option("количество", "Сколько треков пропустить")] long count = 1)
        {
            var playlist = Bot.GetPlaylist(ctx.Guild);
            if(playlist != null)
            {
                if(!playlist.Any() && playlist.CurrentTrack == null)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Очередь пуста!"));
                    return;
                }
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"{playlist.CurrentTrack.Title} пропущен.", ctx.Member));
                await playlist.PlayNext((int)count);
            }
        }

        [SlashCommand("queue", "Показать текущую очередь воспроизведения")]
        public async Task Queue(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var playlist = Bot.GetPlaylist(ctx.Guild);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embeds.QueueEmbed(playlist, ctx.Member)));
        }

        [SlashCommand("remove", "Убрать трек из очереди")]
        public async Task Remove(InteractionContext ctx, [Option("Номер", "Номер удаляемого трека в очереди")]long position)
        {
            var playlist = Bot.GetPlaylist(ctx.Guild);
            if(playlist != null && playlist.Count < position)
            {
                playlist.Remove((int)position);
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"`{position}.` {playlist.CurrentTrack} удален.", ctx.Member));
            }
            else
            {
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"Невозможно удалить трек `{position}`. Неверный номер.", ctx.Member));
            }
        }

        [SlashCommand("loop", "Зациклить трек")]
        public async Task Loop(InteractionContext ctx)
        {
            var playlist = Bot.GetPlaylist(ctx.Guild);
            if(playlist != null && playlist.CurrentTrack != null)
            {
                playlist.ChangeRepeat();
                await ctx.CreateResponseAsync(Embeds.UniEmbed($"Трек {(playlist.IsRepeat ? "зациклен" : "расциклен")}", ctx.Member));
            }
        }


    }
}
