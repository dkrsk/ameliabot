using DnKR.AmeliaBot;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DnKR.AmeliaBot;

public class CommonContext
{
    public delegate Task RespondOperation(DiscordEmbed embed, bool ephemeral = false, DiscordComponent[]? components = null);
    public delegate Task EditResponseAsyncOperation(DiscordWebhookBuilder webhookBuilder, IEnumerable<DiscordAttachment>? attachments = null);
    public delegate Task DeferOperation(bool ephemeral = false);

    public DiscordGuild Guild { get; private set; }
    public DiscordMember Member { get; private set; }
    public DiscordChannel Channel { get; private set; }

    public RespondOperation RespondAsync;
    public EditResponseAsyncOperation? EditResponseAsync;
    public DeferOperation? DeferAsync;

    public CommonContext(InteractionContext context)
    {
        this.Guild = context.Guild;
        this.Member = context.Member;
        this.Channel = context.Channel;
        this.RespondAsync = (DiscordEmbed embed, bool e, DiscordComponent[]? c) => context.CreateResponseAsync(embed, e);
        this.EditResponseAsync= context.EditResponseAsync;
        this.DeferAsync = context.DeferAsync;
    }

    public CommonContext(CommandContext context)
    {
        this.Guild = context.Guild;
        this.Member = context.Member ?? (DiscordMember)Bot.Client.CurrentUser;
        this.Channel = context.Channel;
        this.RespondAsync = (DiscordEmbed embed, bool e, DiscordComponent[]? components) =>
        {
            var msg = new DiscordMessageBuilder().AddEmbed(embed);
            if (components != null)
                msg.AddComponents(components);
            return context.RespondAsync(msg);
        };
    }
}
