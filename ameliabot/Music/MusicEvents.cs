using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Lavalink4NET.Tracks;

namespace DnKR.AmeliaBot.Music;

public static class MusicEvents
{
    public static async Task VoiceStateUpdated(DiscordClient client, VoiceStateUpdateEventArgs args) //skipcq: CS-R1073
    {
        if(args.Before?.Channel.Users.Count == 1)
        {
            Thread checkAndLeave = new(async () =>
            {
                Thread.Sleep(15000);
                var users = args.Before.Channel.Users;
                if(users.Count == 1 && users[0].IsCurrent)
                {
                    var player = await Bot.AudioService.Players.GetPlayerAsync(args.Guild.Id);
                    if (player != null)
                        await player.DisconnectAsync().ConfigureAwait(false);
                        await player.DisposeAsync().ConfigureAwait(false);
                    return;
                }
            });

            checkAndLeave.Start();
        }
    }

    public static async Task ButtonSearchClicked(DiscordClient client, ComponentInteractionCreateEventArgs args) //skipcq: CS-R1073
    {
        if(!args.Id.StartsWith("btn_srch"))
        {
            return;
        }

        if (int.TryParse(args.Id[^1].ToString(), out int index))
        {
            var playlist = await Bot.AudioService.Players.GetPlayerAsync<GuildPlaylist>(args.Guild.Id);
            if (playlist != null)
            {
                LavalinkTrack? track = playlist.SearchResults[index-1];
                if (track != null)
                {
                    await playlist.PlayAsync(track).ConfigureAwait(false);
                    playlist.SearchResults = new LavalinkTrack?[5];

                    await args.Message.DeleteAsync();
                    await args.Interaction.Channel.SendMessageAsync(MusicEmbeds.TrackAdded(track, (DiscordMember)args.User));
                }
            }
        }
            
    }

}
