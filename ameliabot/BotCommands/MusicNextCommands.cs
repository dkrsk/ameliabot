using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DnKR.AmeliaBot.BotCommands;

public class MusicNextCommands : BaseCommandModule
{
    [Command("join"), Aliases(new[] { "ощшт" })]
    public async Task JoinCommand(CommandContext ctx)
    {
        await MusicCommands.JoinAsync(new CommonContext(ctx));
    }

    [Command("leave"), Aliases(new[] { "dc", "вс", "дуфму" })]
    public async Task LeaveCommand(CommandContext ctx)
    {
        await MusicCommands.LeaveAsync(new CommonContext(ctx));
    }

    [Command("play"), Aliases(new[] { "p", "з", "здфн" })]
    public async Task PlayCommand(CommandContext ctx, string query)
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query);
    }

    [Command("search"), Aliases(new[] { "sc", "ыс", "ыуфкср" })]
    public async Task SearchCommand(CommandContext ctx, string query)
    {
        await MusicCommands.SearchAsync(new CommonContext(ctx), query);
    }

    [Command("skip"), Aliases(new[] { "s", "fs", "ы", "аы", "ылшз"})]
    public async Task SkipCommand(CommandContext ctx, long count = 1)
    {
        await MusicCommands.SkipAsync(new CommonContext(ctx), count);
    }

    [Command("queue"), Aliases(new[] { "q", "й", "йгугу"})]
    public async Task QueueCommand(CommandContext ctx)
    {
        await MusicCommands.QueueAsync(new CommonContext(ctx));
    }

    [Command("remove"), Aliases(new[] { "rm", "кь", "куьщму"})]
    public async Task RemoveCommand(CommandContext ctx, long position)
    {
        await MusicCommands.RemoveAsync(new CommonContext(ctx), position);
    }

    [Command("loop"), Aliases(new[] { "l", "д", "дщщз"})]
    public async Task Loop(CommandContext ctx)
    {
        await MusicCommands.LoopAsync(new CommonContext(ctx));
    }
}
