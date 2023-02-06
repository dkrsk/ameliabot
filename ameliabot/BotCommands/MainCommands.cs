using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnKR.AmeliaBot.BotCommands
{
    public class MainCommands : ApplicationCommandModule
    {

        [SlashCommand("ping", "If Amelia is online, return Pong!")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }
    }
}
