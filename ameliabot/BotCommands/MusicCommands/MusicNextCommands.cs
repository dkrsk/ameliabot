using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Lavalink4NET;

using Microsoft.Extensions.DependencyInjection;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public partial class MusicNextCommands : BaseCommandModule
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
        await MusicCommands.JoinAsync(new NextContext(ctx));
    }

    [Command("leave"), Aliases("dc", "вс", "дуфму")]
    public async Task LeaveCommandAsync(CommandContext ctx)
    {
        await MusicCommands.LeaveAsync(new NextContext(ctx));
    }

    [Command("play"), Aliases("p", "з", "здфн")]
    public async Task PlayCommandAsync(CommandContext ctx, [RemainingText] string query = "pause")
    {
        await MusicCommands.PlayAsync(new NextContext(ctx), query, false);
    }
    
    [Command("playtop"), Aliases("pt", "зе", "здфнещз")]
    public async Task PlayTopCommandAsync(CommandContext ctx, [RemainingText] string query)
    {
        await MusicCommands.PlayAsync(new NextContext(ctx), query, true);
    }

    [Command("search"), Aliases("sc", "ыс", "ыуфкср")]
    public async Task SearchCommandAsync(CommandContext ctx,[RemainingText] string query)
    {
        await MusicCommands.SearchAsync(new NextContext(ctx), query);
    }

    [Command("skip"), Aliases("s", "fs", "ы", "аы", "ылшз")]
    public async Task SkipCommandAsync(CommandContext ctx, long count = 1)
    {
        await MusicCommands.SkipAsync(new NextContext(ctx), count);
    }

    [Command("queue"), Aliases("q", "й", "йгугу")]
    public async Task QueueCommandAsync(CommandContext ctx)
    {
        await MusicCommands.QueueAsync(new NextContext(ctx));
    }

    [Command("remove"), Aliases("rm", "кь", "куьщму")]
    public async Task RemoveCommandAsync(CommandContext ctx, long position)
    {
        await MusicCommands.RemoveAsync(new NextContext(ctx), position);
    }

    [Command("loop"), Aliases("l", "д", "дщщз")]
    public async Task LoopCommandAsync(CommandContext ctx)
    {
        await MusicCommands.LoopAsync(new NextContext(ctx));
    }

    [Command("clear"), Aliases("cl", "сд", "сдуфк")]
    public async Task ClearCommandAsync(CommandContext ctx)
    {
        await MusicCommands.ClearAsync(new NextContext(ctx));
    }

    [Command("pause"), Aliases("зфгыу")]
    public async Task PauseCommandAsync(CommandContext ctx)
    {
        await MusicCommands.PauseAsync(new NextContext(ctx));
    }

    [Command("playskip"), Aliases("ps", "зы", "здфнылшз")]
    public async Task PlaySkipCommandAsync(CommandContext ctx, [RemainingText] string query)
    {
        await MusicCommands.PlaySkipAsync(new NextContext(ctx), query);
    }

    [Command("previous"), Aliases("prev", "зкум", "зкумшщгы")]
    public async Task PreviousCommandAsync(CommandContext ctx)
    {
        await MusicCommands.PlayPreviousAsync(new NextContext(ctx));
    }

    [Command("lofi"), Aliases("дщаш")]
    public async Task PlayLofiCommand(CommandContext ctx)
    {
        await PlayCommandAsync(ctx, query: "https://www.youtube.com/watch?v=jfKfPfyJRdk");
    }
}
