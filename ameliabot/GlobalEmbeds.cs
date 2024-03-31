using DSharpPlus.Entities;

namespace DnKR.AmeliaBot;

public static class GlobalEmbeds
{
    public static DiscordColor AmeliaColor { get; } = new DiscordColor("#fb3d1c");

    public static DiscordEmbed UniEmbed(string query, DiscordMember? reqby)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby?.Color ?? AmeliaColor,
            Title = query
        };
        return builder.Build();
    }

    private const string shortErrorMessage = "Ой, что-то сломалось >.<";
    public static DiscordEmbed ShortErrorEmbed(DiscordMember? reqby = null)
    {
        var builder = new DiscordEmbedBuilder()
        {
            Color = reqby?.Color ?? AmeliaColor,
            Title = shortErrorMessage
        };
        return builder.Build();
    }

    public static DiscordEmbed DetailedEmbed(DetailedEmbedContent content) 
    {
        if (content.GetHashCode() == DetailedEmbedContent.Empty.GetHashCode())
            throw new ArgumentException("At least one of the arguments must not be null");

        var builder = new DiscordEmbedBuilder()
        {
            Color = content.ReqBy?.Color ?? AmeliaColor,
            Title = content.Title,
            Description = content.Description,
        }
        .WithFooter(text: content.Footer);
        return builder.Build();
    }

    public struct DetailedEmbedContent
    {
        public string? Title { get; init; }
        public DiscordMember? ReqBy { get; init; }
        public string? Description { get; init; }
        public string? Footer { get; init; }

        public static DetailedEmbedContent Empty { get; } = new();
    }
}
