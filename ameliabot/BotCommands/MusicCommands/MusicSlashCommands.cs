using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public class MusicSlashCommands : ApplicationCommandModule
{
    [SlashCommand("join", "Подключиться к твоему голосовому каналу")]
    public async Task JoinCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.JoinAsync(new CommonContext(ctx));
    }

    [SlashCommand("leave", "Покинуть голосовой канал")]
    public async Task LeaveCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.LeaveAsync(new CommonContext(ctx));
    }

    [SlashCommand("play", "Добавить трек в очередь")]
    public async Task PlayCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query);
    }

    [SlashCommand("search", "Выбрать трек из первых 10 по поиску")]
    public async Task SearchCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.SearchAsync(new CommonContext(ctx), query);
    }

    [SlashCommand("skip", "Пропустить текущий трек")]
    public async Task SkipCommandAsync(InteractionContext ctx, [Option("количество", "Сколько треков пропустить")] long count = 1)
    {
        await MusicCommands.SkipAsync(new CommonContext(ctx), count);
    }

    [SlashCommand("queue", "Показать текущую очередь воспроизведения")]
    public async Task QueueCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.QueueAsync(new CommonContext(ctx));
    }

    [SlashCommand("remove", "Убрать трек из очереди")]
    public async Task RemoveCommandAsync(InteractionContext ctx, [Option("Номер", "Номер удаляемого трека в очереди")] long position)
    {
        await MusicCommands.RemoveAsync(new CommonContext(ctx), position);
    }

    [SlashCommand("loop", "Зациклить трек")]
    public async Task LoopCommandAsync(InteractionContext ctx)
    {
        await MusicCommands.LoopAsync(new CommonContext(ctx));
    }
}
