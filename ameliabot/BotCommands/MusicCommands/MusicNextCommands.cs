using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public class MusicNextCommands : BaseCommandModule
{
    [Command("join"), Aliases("ощшт")]
    public async Task JoinCommandAsync(CommandContext ctx)
    {
        await MusicCommands.JoinAsync(new CommonContext(ctx));
    }

    [Command("leave"), Aliases("dc", "вс", "дуфму")]
    public async Task LeaveCommandAsync(CommandContext ctx)
    {
        await MusicCommands.LeaveAsync(new CommonContext(ctx));
    }

    [Command("play"), Aliases("p", "з", "здфн")]
    public async Task PlayCommandAsync(CommandContext ctx, string query)
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query);
    }

    [Command("search"), Aliases("sc", "ыс", "ыуфкср")]
    public async Task SearchCommandAsync(CommandContext ctx, string query)
    {
        await MusicCommands.SearchAsync(new CommonContext(ctx), query);
    }

    [Command("skip"), Aliases("s", "fs", "ы", "аы", "ылшз")]
    public async Task SkipCommandAsync(CommandContext ctx, long count = 1)
    {
        await MusicCommands.SkipAsync(new CommonContext(ctx), count);
    }

    [Command("queue"), Aliases("q", "й", "йгугу")]
    public async Task QueueCommandAsync(CommandContext ctx)
    {
        await MusicCommands.QueueAsync(new CommonContext(ctx));
    }

    [Command("remove"), Aliases("rm", "кь", "куьщму")]
    public async Task RemoveCommandAsync(CommandContext ctx, long position)
    {
        await MusicCommands.RemoveAsync(new CommonContext(ctx), position);
    }

    [Command("loop"), Aliases("l", "д", "дщщз")]
    public async Task LoopCommandAsync(CommandContext ctx)
    {
        await MusicCommands.LoopAsync(new CommonContext(ctx));
    }
}
