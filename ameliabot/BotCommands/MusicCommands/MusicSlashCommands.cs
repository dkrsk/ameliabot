using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public partial class MusicSlashCommands : ApplicationCommandModule
{
    private readonly MusicCommands MusicCommands;

    public MusicSlashCommands(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        this.MusicCommands = MusicCommands.GetInstance(serviceProvider.GetRequiredService<IAudioService>());
    }

    [SlashCommand("join", "Подключиться к твоему голосовому каналу")]
    public async Task JoinCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.JoinAsync(new SlashContext(ctx));
    }

    [SlashCommand("leave", "Покинуть голосовой канал")]
    public async Task LeaveCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.LeaveAsync(new SlashContext(ctx));
    }

    [SlashCommand("play", "Добавить трек в очередь")]
    public async Task PlayCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query = "pause")
    {
        await MusicCommands.PlayAsync(new SlashContext(ctx), query, false);
    }
    
    [SlashCommand("playtop", "Добавить трек в начало очереди")]
    public async Task PlayTopCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.PlayAsync(new SlashContext(ctx), query, true);
    }

    [SlashCommand("search", "Выбрать трек из первых 10 по поиску")]
    public async Task SearchCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.SearchAsync(new SlashContext(ctx), query);
    }

    [SlashCommand("skip", "Пропустить текущий трек")]
    public async Task SkipCommandAsync(InteractionContext ctx, [Option("количество", "Сколько треков пропустить")] long count = 1)
    {
        await MusicCommands.SkipAsync(new SlashContext(ctx), count);
    }

    [SlashCommand("queue", "Показать текущую очередь воспроизведения")]
    public async Task QueueCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.QueueAsync(new SlashContext(ctx));
    }

    [SlashCommand("remove", "Убрать трек из очереди")]
    public async Task RemoveCommandAsync(InteractionContext ctx, [Option("Номер", "Номер удаляемого трека в очереди")] long position)
    {
        await MusicCommands.RemoveAsync(new SlashContext(ctx), position);
    }

    [SlashCommand("loop", "Зациклить трек")]
    public async Task LoopCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.LoopAsync(new SlashContext(ctx));
    }

    [SlashCommand("clear", "Очистить очередь")]
    public async Task ClearCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.ClearAsync(new SlashContext(ctx));
    }

    [SlashCommand("pause", "Приостоновить/продолжить воспроизведение")]
    public async Task PauseCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.PauseAsync(new SlashContext(ctx));
    }

    [SlashCommand("playskip", "Добавить трек в очередь и пропустить текущий")]
    public async Task PlaySkipCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.PlaySkipAsync(new SlashContext(ctx), query);
    }

    [SlashCommand("prev", "Вернуться к предыдущему треку")]
    public async Task PrevCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.PlayPreviousAsync(new SlashContext(ctx));
    }

    [SlashCommand("lofi", "Включить лофай-герл")]
    public async Task PlayLofiCommand(InteractionContext ctx)
    {
        await PlayCommandAsync(ctx, query: "https://www.youtube.com/watch?v=jfKfPfyJRdk");
    }

}
