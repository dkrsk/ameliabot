using DSharpPlus.Entities;
using DnKR.PastaParse;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public class ChatSlashCommands : ApplicationCommandModule
{
    [SlashCommand("pasta", "Пишет случайную пасту с copypastas.ru")]
    public async Task SayPastaAsync(InteractionContext ctx)
    {
        await ctx.DeferAsync();
        var pasta = await PastaParser.GetFilteredPastaAsync("ASCII", "стрим", "стример", "саб", "твич");
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(pasta.Text));
    }
}

public class ChatNextCommands : BaseCommandModule
{
    [Command("pasta"), Aliases("зфыеф", "паста")]
    public async Task SayPastaAsync(CommandContext ctx)
    {
        var pasta = await PastaParser.GetFilteredPastaAsync("ASCII", "стрим", "стример", "саб", "твич");
        await ctx.RespondAsync(pasta.Text);
    }
}
