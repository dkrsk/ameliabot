using DSharpPlus.Entities;

namespace DnKR.AmeliaBot.BotCommands;

public static class GlobalEmbeds
{

    public static DiscordEmbed UniEmbed(string query, DiscordMember reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby.Color,
            Title = query
        };
        return builder.Build();
    }
}