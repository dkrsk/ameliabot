using DnKR.AmeliaBot.Music;
using DnKR.VkAudioExtractor;
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

    [ProviderChecker]
    private bool VkProviderCheck(string query)
    {
        return false;
    }

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

    public async Task VkPlayAsync(CommonContext ctx, string query, bool playTop)
    {
        if (!isVkAuthSuccess) await AuthVkAsync();
        var playlist = await GetPlaylistAsync(ctx);
        if (playlist is null) return;

        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"Поиск по Very Kool music работает в бета режиме!", ctx.Member));

        var searchResult = await vk.SearchAsync(query, 1,0);

        if (!searchResult.IsSucces)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"По запросу {query} ничего не нашлось.", ctx.Member));
            return;
        }
        
        var a = await audioService.Tracks.LoadTrackAsync(searchResult.Tracks[0].Uri, TrackSearchMode.None);

        var track = new LavalinkTrack
        {
            Author = searchResult.Tracks[0].Artist,
            Uri = new Uri("http://www.verycoolmusic.com/"),//new Uri(searchResult.Tracks[0].ShareUrl),
            ArtworkUri = new Uri(searchResult.Tracks[0].Thumb),
            Title = searchResult.Tracks[0].Title,
            Identifier = a.Identifier,
            Duration = TimeSpan.FromSeconds(searchResult.Tracks[0].Duration),
            SourceName = a.SourceName,
            ProbeInfo = a.ProbeInfo
        };
        
        await ctx.RespondEmbedAsync(MusicEmbeds.TrackAdded(track, ctx.Member));

        if (playTop)
        {
            await playlist.Queue.InsertAsync(0, new TrackQueueItem(track));
        }
        else
            await playlist.PlayAsync(track);
    }

}
