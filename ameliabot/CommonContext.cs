using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot;

public class CommonContext
{
    public delegate Task RespondEmbedOperation(DiscordEmbed embed, bool ephemeral = false, DiscordComponent[]? components = null);
    public delegate Task RespondTextOperation(string content, bool ephemeral = false);
    public delegate Task EditResponseAsyncOperation(DiscordWebhookBuilder webhookBuilder, IEnumerable<DiscordAttachment>? attachments = null);
    public delegate Task DeferOperation(bool ephemeral = false);

    public DiscordGuild Guild { get; private set; }
    public DiscordMember Member { get; private set; }
    public DiscordChannel Channel { get; private set; }

    public RespondEmbedOperation RespondEmbedAsync;
    public RespondTextOperation RespondTextAsync;
    public EditResponseAsyncOperation? EditResponseAsync;
    public DeferOperation? DeferAsync;

    public CommonContext(InteractionContext context)
    {
        this.Guild = context.Guild;
        this.Member = context.Member;
        this.Channel = context.Channel;
        this.RespondEmbedAsync = (DiscordEmbed embed, bool e, DiscordComponent[]? c) => context.CreateResponseAsync(embed, e);
        this.RespondTextAsync = context.CreateResponseAsync;
        this.EditResponseAsync= context.EditResponseAsync;
        this.DeferAsync = context.DeferAsync;
    }

    public CommonContext(CommandContext context)
    {
        this.Guild = context.Guild;
        this.Member = context.Member ?? (DiscordMember)context.Client.CurrentUser;
        this.Channel = context.Channel;
        this.RespondEmbedAsync = (DiscordEmbed embed, bool e, DiscordComponent[]? components) =>
        {
            var msg = new DiscordMessageBuilder().AddEmbed(embed);
            if (components != null)
                msg.AddComponents(components);
            return context.RespondAsync(msg);
        };
        this.RespondTextAsync = (string content, bool ephemeral) => context.RespondAsync(content);
    }
}
