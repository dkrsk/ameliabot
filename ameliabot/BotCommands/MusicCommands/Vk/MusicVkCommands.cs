using DnKR.AmeliaBot.Music;
using DnKR.VkAudioExtractor;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using System;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

partial class MusicCommands
{
    VkAudio vk = new VkAudio();
    bool isVkAuthSuccess = false;

    private async Task AuthVkAsync()
    {
        if (!isVkAuthSuccess)
        {
            try
            {
                await vk.AuthWithTokenAsync(Environment.GetEnvironmentVariable("VKToken"), Environment.GetEnvironmentVariable("VKSecret"));
                isVkAuthSuccess = true;
            }
            catch (Exception) {  }
        }
    }

    [ProviderLoader]
    private async ValueTask<(LavalinkTrack?, bool)> LoadVkAsync(CommonContext ctx, GuildPlaylist playlist, string query)
    {
        if (!(query.StartsWith("vksearch:") || VkRegex().Match(query).Success)) return (null, false);

        if (!isVkAuthSuccess) await AuthVkAsync();

        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Поиск по Very Kool music работает в бета режиме!", ctx.Member));

        var searchResult = await vk.SearchAsync(query.Substring(9), 1,0);

        if (!searchResult.IsSucces) return (null, false);
        
        var loadedTrack = await audioService.Tracks.LoadTrackAsync(searchResult.Tracks[0].Uri, TrackSearchMode.None);
        
        var track = new LavalinkTrack
        {
            Author = searchResult.Tracks[0].Artist,
            Uri = new Uri("http://www.verycoolmusic.com/"),//new Uri(searchResult.Tracks[0].ShareUrl),
            ArtworkUri = new Uri(searchResult.Tracks[0].Thumb),
            Title = searchResult.Tracks[0].Title,
            Identifier = loadedTrack.Identifier,
            Duration = TimeSpan.FromSeconds(searchResult.Tracks[0].Duration),
            SourceName = loadedTrack.SourceName,
            ProbeInfo = loadedTrack.ProbeInfo
        };

        return (track, true);
    }

}
