using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace DnKR.AmeliaBot.Music;

public static class Embeds
{
    public static DiscordEmbed NowPlaying(LavalinkTrack track, LavalinkTrack? next)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = Bot.Client.CurrentUser.BannerColor ?? DiscordColor.Gray,
            Title = "Сейчас играет :musical_note:",
            Url = track.Uri.ToString(),
            Description = $"[{track.Title}]({track.Uri})"
        };
        builder.WithThumbnail($"https://i3.ytimg.com/vi/{track.Identifier}/maxresdefault.jpg");
        builder.AddField("Автор: ", track.Author, false);
        builder.AddField("Длительность: ", track.Length.ToString(), false);
        builder.AddField("Дальше: ", next != null ? next.Title : "Ничего", false);

        return builder.Build();
    }

    public static DiscordEmbed TrackAdded(LavalinkTrack track, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Title = "Трек добавлен!",
            Url = track.Uri.ToString(),
            Description = $"[{track.Title}]({track.Uri})"
        };
        builder.WithThumbnail($"https://i3.ytimg.com/vi/{track.Identifier}/maxresdefault.jpg");
        builder.AddField("Автор: ", track.Author, true);
        builder.AddField("Длительность: ", track.Length.ToString(), false);

        return builder.Build();
    }

    public static DiscordEmbed QueueEmbed(GuildPlaylist? playlist, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Url = "https://github.com/dkrsk"
        };
        string desc = string.Empty;
        if (playlist != null && playlist.Count > 0)
        {
            builder.Title = $"Очередь для {playlist.Channel.Name}";
            for (int i = 0; i < playlist.Count; i++)
            {
                var track = playlist.At(i);
                desc += $"{i + 1}. {track.Title} | {track.Length}\n";
            }
        }
        else
        {
            desc = "Очередь пуста!";
        }

        builder.WithDescription(desc);
        return builder.Build();
    }

    public static Tuple<DiscordEmbed, DiscordComponent[]> SearchEmbed(LavalinkTrack[] tracks, DiscordMember reqby)
    {
        var embedBuilder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Title = "Результаты поиска",
        };
        string desc = string.Empty;
        for (int i = 0; i <= 4; i++)
        {
            desc += $"{i + 1}. {tracks[i].Title}\n";
        }
        embedBuilder.WithDescription(desc);

        var buttons = new DiscordComponent[]
        {
            new DiscordButtonComponent(ButtonStyle.Primary, "btn_srch_1", "1"),
            new DiscordButtonComponent(ButtonStyle.Primary, "btn_srch_2", "2"),
            new DiscordButtonComponent(ButtonStyle.Primary, "btn_srch_3", "3"),
            new DiscordButtonComponent(ButtonStyle.Primary, "btn_srch_4", "4"),
            new DiscordButtonComponent(ButtonStyle.Primary, "btn_srch_5", "5"),
        };

        return Tuple.Create(embedBuilder.Build(), buttons);
    }

    public static DiscordEmbed UniEmbed(string query, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Title = query
        };
        return builder.Build();
    }
}
