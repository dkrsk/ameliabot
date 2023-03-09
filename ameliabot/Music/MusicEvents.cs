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
    }

    public static async Task VoiceStateUpdated(DiscordClient client, VoiceStateUpdateEventArgs args)
    {
        if (args.Before == null) return;
        var playlist = Bot.GetPlaylist(args.Guild);
        if (playlist != null)
        {
            if (args.User.IsCurrent && Bot.Lava.node.GetGuildConnection(args.Guild) != null && args.After.Channel == null)
            {
                await Bot.RemovePlaylistAsync(args.Guild);
                return;
            }
            if(args.Before.Channel.Users.Count == 1)
            {
                Thread checkAndLeave = new(async () =>
                {
                    Thread.Sleep(15000);
                    var users = args.Before.Channel.Users;
                    if(users.Count == 1 && users[0].IsCurrent)
                    {
                        await Bot.RemovePlaylistAsync(args.Guild);
                        await Bot.Lava.node.GetGuildConnection(args.Guild).DisconnectAsync();
                        return;
                    }
                });

                checkAndLeave.Start();
            }
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
