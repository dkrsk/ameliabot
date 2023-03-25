using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public class ChatSlashCommands : ApplicationCommandModule
{
    [SlashCommand("pasta", "Пишет случайную пасту с copypastas.ru")]
    public async Task SayPastaAsync(InteractionContext ctx)
    {
        await ChatCommands.SayPastaAsync(new CommonContext(ctx));
    }

    [SlashCommand("coin", "Подбрасывает монетку")]
    public async Task FlipCoinAsync(InteractionContext ctx)
    {
        await ChatCommands.FlipCoinAsync(new CommonContext(ctx));
    }

    [SlashCommand("rand", "Выдает случайное число в заданных пределах")]
    public async Task GetRandomAsync(InteractionContext ctx,
                                     [Option("минимальное", "Минимальное значение случайного числа")] long min = 0,
                                     [Option("максимальное", "Максимальное значение случайного числа")] long max = 101)
    {
        await ChatCommands.GetRandomAsync(new CommonContext(ctx), (int)min, (int)max);
    }
}
