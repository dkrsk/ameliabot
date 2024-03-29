﻿using DSharpPlus.Entities;
using DnKR.PastaParse;

namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public static class ChatCommands
{
    public static async Task SayPastaAsync(CommonContext ctx)
    {
        if (ctx.DeferAsync != null)
            await ctx.DeferAsync(false);

        string? pasta;
        try
        {
            pasta = (await PastaParser.GetFilteredPastaAsync("ASCII", "стрим", "стример", "саб", "твич")).Text;
        }
        catch (Exception e)
        {
            pasta = "Админ copypastas.ru лучший!!!!)))))";
        }
        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(pasta));
        else
            await ctx.RespondTextAsync(pasta);
    }

    public static async Task FlipCoinAsync(CommonContext ctx)
    {
        Random rand = new(DateTime.UtcNow.Millisecond);
        if(rand.Next(2) == 1)
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(":coin: Решка!", ctx.Member));
        }
        else
        {
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed(":eagle: Орёл!", ctx.Member));
        }
    }

    public static async Task GetRandomAsync(CommonContext ctx, int min, int max)
    {
        if (max < 0 || min < 0 || max > Int32.MaxValue)
            await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed("Числа должны быть натуральными и в нормальных пределах)", ctx.Member));

        Random rand = new(DateTime.UtcNow.Millisecond);
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{ctx.Member.DisplayName} получает число {rand.Next(min, max)}", ctx.Member));
    }

    public static async Task GetRandomAsync(CommonContext ctx)
    {
        await GetRandomAsync(ctx, 0, 101);
    }
}
