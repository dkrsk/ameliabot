using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace DnKR.AmeliaBot.Music;

public static class Events
{
    public static async Task PlaybackFinished(LavalinkGuildConnection lava, TrackFinishEventArgs args)
    {
        var playlist = Bot.GetPlaylist(lava.Guild);
        if (playlist != null)
        {
            await playlist.PlayNext();
                
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
