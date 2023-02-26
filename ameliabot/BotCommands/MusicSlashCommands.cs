using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

using DnKR.AmeliaBot.Music;

namespace DnKR.AmeliaBot.BotCommands;

public class MusicSlashCommands : ApplicationCommandModule
{
    [SlashCommand("join", "Подключиться к твоему голосовому каналу")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        await MusicCommands.JoinAsync(new CommonContext(ctx));
    }

    [SlashCommand("leave", "Покинуть голосовой канал")]
    public async Task LeaveCommand(InteractionContext ctx)
    {
        await MusicCommands.LeaveAsync(new CommonContext(ctx));
    }

    [SlashCommand("play", "Добавить трек в очередь")]
    public async Task PlayCommand(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.PlayAsync(new CommonContext(ctx), query);
    }

    [SlashCommand("search", "Выбрать трек из первых 10 по поиску")]
    public async Task SearchCommand(InteractionContext ctx, [Option("название", "Название трека")] string query)
    {
        await MusicCommands.SearchAsync(new CommonContext(ctx), query);
    }

    [SlashCommand("skip", "Пропустить текущий трек")]
    public async Task SkipCommand(InteractionContext ctx, [Option("количество", "Сколько треков пропустить")] long count = 1)
    {
        await MusicCommands.SkipAsync(new CommonContext(ctx), count);
    }

    [SlashCommand("queue", "Показать текущую очередь воспроизведения")]
    public async Task QueueCommand(InteractionContext ctx)
    {
        await MusicCommands.QueueAsync(new CommonContext(ctx));
    }

    [SlashCommand("remove", "Убрать трек из очереди")]
    public async Task RemoveCommand(InteractionContext ctx, [Option("Номер", "Номер удаляемого трека в очереди")] long position)
    {
        await MusicCommands.RemoveAsync(new CommonContext(ctx), position);
    }

    [SlashCommand("loop", "Зациклить трек")]
    public async Task Loop(InteractionContext ctx)
    {
        await MusicCommands.LoopAsync(new CommonContext(ctx));
    }
}
