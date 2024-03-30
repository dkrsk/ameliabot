using DSharpPlus;
using DSharpPlus.Entities;
using Lavalink4NET.Tracks;

namespace DnKR.AmeliaBot.Music;

public static class MusicEmbeds
{
    private const string githubUrl = "https://github.com/dkrsk/ameliabot0";
    private static string GetDuration(LavalinkTrack? track)
    {
        if(track is null) return "Unknown";
        return track.IsLiveStream ? "Live" : track.Duration.ToString();
    }

    public static DiscordEmbed NowPlaying(LavalinkTrack track)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = GlobalEmbeds.AmeliaColor,
            Title = "Сейчас играет :musical_note:",
            Url = track.Uri?.ToString() ?? githubUrl,
            Description = $"[{track.Title}]({track.Uri})"
        }
        .WithThumbnail(track.ArtworkUri)
        .AddField("Автор: ", track.Author, false)
        .AddField("Длительность: ",
            GetDuration(track),
            false);
        
        return builder.Build();
    }

    public static DiscordEmbed TrackAdded(LavalinkTrack track, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Title = "Трек добавлен!",
            Url = track.Uri?.ToString() ?? githubUrl,
            Description = $"[{track.Title}]({track.Uri})"
        }
        .WithThumbnail(track.ArtworkUri)
        .AddField("Автор: ", track.Author, true)
        .AddField("Длительность: ",
            GetDuration(track),
            false);
        
        return builder.Build();
    }

    public static DiscordEmbed QueueEmbed(GuildPlaylist playlist, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Url = githubUrl
        };
        string desc = string.Empty;


        if(playlist.CurrentTrack != null)
        {
            builder.AddField("Сейчас играет :musical_note:", $"[{playlist.CurrentTrack.Title}]({playlist.CurrentTrack.Uri})");

            if (playlist.Queue.Count > 0)
            {
                builder.Title = $"Очередь для {playlist.Channel.Name}";
                for (int i = 0; i < playlist.Queue.Count; i++)
                {
                    var track = playlist.Queue[i].Track;
                    desc += $"{i + 1}. {track?.Uri?.ToString() ?? "Unknown"} | {GetDuration(track)}";
                }
            }
            else
            {
                desc = "Дальше ничего нет!";
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

    public static DiscordEmbed EmptyQueueEmbed(DiscordMember reqby) => GlobalEmbeds.UniEmbed("Ничего не воспроизводится!", reqby);
}
