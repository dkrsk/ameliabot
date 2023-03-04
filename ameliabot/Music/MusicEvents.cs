using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Emzi0767.Utilities;

namespace DnKR.AmeliaBot.Music;

public static class MusicEvents
{
    public static async Task PlaybackFinished(LavalinkGuildConnection lava, TrackFinishEventArgs args)
    {
        if (lava.Channel == null) return;
        var playlist = Bot.GetPlaylist(lava.Guild);
        if (playlist != null)
        {
            await playlist.PlayNextAsync();
            return;
        }
        if (lava.Node.GetGuildConnection(lava.Guild).Channel.Users.Count == 1)
        {
            await Bot.RemovePlaylistAsync(lava.Guild);
        }
    }

    public static async Task VoiceStateUpdated(DiscordClient client, VoiceStateUpdateEventArgs args)
    {
        if (!args.User.IsCurrent) return;
        var playlist = Bot.GetPlaylist(args.Guild);
        if (playlist != null)
        {
            await Bot.RemovePlaylistAsync(args.Guild);
        }
    }

    public static async Task ButtonSearchClicked(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        if(!args.Id.StartsWith("btn_srch"))
        {
            return;
        }

        if (int.TryParse(args.Id[^1].ToString(), out int index))
        {
            var playlist = Bot.GetPlaylist(args.Guild);
            if (playlist != null)
            {
                LavalinkTrack? track = playlist.SearchResults[index-1];
                if (track != null)
                {
                    await playlist.AddAsync(track);
                    playlist.SearchResults = new LavalinkTrack?[5];

                    await args.Message.DeleteAsync();
                    await args.Interaction.Channel.SendMessageAsync(Embeds.TrackAdded(track, (DiscordMember)args.User));
                }
            }
        }
            
    }

}
