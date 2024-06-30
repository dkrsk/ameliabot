
using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public partial class MusicSlashCommands
{
    [SlashCommand("vkplay", "Добавить трек в очередь (из Very Kool)")]
    public async Task PlayVkCommandAsync(InteractionContext ctx, [Option("название", "Название трека")] string query = "pause")
    {
        await MusicCommands.PlayAsync(new SlashContext(ctx), "vksearch:"+query, false);
    }
}
