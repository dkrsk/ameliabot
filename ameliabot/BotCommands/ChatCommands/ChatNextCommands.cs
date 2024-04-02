using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;

namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public class ChatNextCommands : BaseCommandModule
{
    [Command("pasta"), Aliases("зфыеф", "паста")]
    public async Task SayPastaAsync(CommandContext ctx)
    {
        await ChatCommands.SayPastaAsync(new NextContext(ctx));
    }

    [Command("coin"), Aliases("сщшт")]
    public async Task FlipCoinAsync(CommandContext ctx)
    {
        await ChatCommands.FlipCoinAsync(new NextContext(ctx));
    }

    [Command("rand")]
    public async Task GetRandomAsync(CommandContext ctx, int minValue, int maxValue)
    {
        await ChatCommands.GetRandomAsync(new NextContext(ctx), minValue, maxValue);
    }

    [Command("rand"), Aliases("кфтв")]
    public async Task GetRandomAsync(CommandContext ctx)
    {
        await ChatCommands.GetRandomAsync(new NextContext(ctx));
    }
}
