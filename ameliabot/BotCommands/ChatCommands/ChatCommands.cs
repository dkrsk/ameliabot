using DSharpPlus.Entities;
using DnKR.PastaParse;

namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public static class ChatCommands
{
    public static async Task SayPastaAsync(CommonContext ctx)
    {
        if (ctx.DeferAsync != null)
            await ctx.DeferAsync(false);

        var pasta = await PastaParser.GetFilteredPastaAsync("ASCII", "стрим", "стример", "саб", "твич");
        if (ctx.EditResponseAsync != null)
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(pasta.Text));
        else
            await ctx.RespondTextAsync(pasta.Text);
    }

    public static async Task FlipCoinAsync(CommonContext ctx)
    {
        Random rand = new Random(DateTime.Now.Millisecond);
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
        Random rand = new Random(DateTime.Now.Millisecond);
        await ctx.RespondEmbedAsync(GlobalEmbeds.UniEmbed($"{ctx.Member.DisplayName} получает число {rand.Next(min, max)}!", ctx.Member));
    }

    public static async Task GetRandomAsync(CommonContext ctx)
    {
        await GetRandomAsync(ctx, 0, 101);
    }
}
