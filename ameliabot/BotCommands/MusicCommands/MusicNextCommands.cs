using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Lavalink4NET;

using Microsoft.Extensions.DependencyInjection;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public class MusicNextCommands : BaseCommandModule
{
    private readonly MusicCommands MusicCommands;

    public MusicNextCommands(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        this.MusicCommands = MusicCommands.GetInstance(serviceProvider.GetRequiredService<IAudioService>());
    }

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
    public async Task PlayCommandAsync(CommandContext ctx, [RemainingText] string query = "pause")
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query, false);
    }
    
    [Command("playtop"), Aliases("pt", "зе", "здфнещз")]
    public async Task PlayTopCommandAsync(CommandContext ctx, [RemainingText] string query)
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query, true);
    }

    [Command("search"), Aliases("sc", "ыс", "ыуфкср")]
    public async Task SearchCommandAsync(CommandContext ctx,[RemainingText] string query)
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

    [Command("clear"), Aliases("cl", "сд", "сдуфк")]
    public async Task ClearCommandAsync(CommandContext ctx)
    {
        await MusicCommands.ClearAsync(new CommonContext(ctx));
    }

    [Command("pause"), Aliases("зфгыу")]
    public async Task PauseCommandAsync(CommandContext ctx)
    {
        await MusicCommands.PauseAsync(new CommonContext(ctx));
    }

    [Command("playskip"), Aliases("ps", "зы", "здфнылшз")]
    public async Task PlaySkipCommandAsync(CommandContext ctx, [RemainingText] string query)
    {
        await MusicCommands.PlaySkipAsync(new CommonContext(ctx), query);
    }

    //previous command
    [Command("previous"), Aliases("prev", "зкум", "зкумшщгы")]
    public async Task PreviousCommandAsync(CommandContext ctx)
    {
        await MusicCommands.PlayPreviousAsync(new CommonContext(ctx));
    }
}
