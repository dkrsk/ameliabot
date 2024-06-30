using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;

namespace DnKR.AmeliaBot.BotCommands.MusicCommands;

public partial class MusicNextCommands
{
    [Command("vkplay"), Aliases("vp","млздфн","мз")]
    public async Task PlayVkCommandAsync(CommandContext ctx, [RemainingText] string query)
    {
        await MusicCommands.PlayAsync(new NextContext(ctx), "vksearch:"+query, false);
    }
}
