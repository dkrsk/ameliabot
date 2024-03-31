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
    public DeferOperation DeferAsync;

    public bool IsDefered { get; private set; } = false;
    public bool IsResposed { get; private set; } = false;

    public CommonContext(InteractionContext context)
    {
        this.Guild = context.Guild;
        this.Member = context.Member;
        this.Channel = context.Channel;
        this.RespondEmbedAsync = (DiscordEmbed embed, bool e, DiscordComponent[]? c) =>
        {
            if (!IsDefered)
            {
                if (!IsResposed) // idk why. maybe its weird
                {
                    IsResposed = true;
                    return context.CreateResponseAsync(embed, e);
                }
                IsResposed = true;
                return context.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed).AsEphemeral(e));
            }

            var msg = new DiscordWebhookBuilder().AddEmbed(embed);
            if (c != null)
                msg.AddComponents(c);
            IsDefered = false;
            return context.EditResponseAsync(msg);
        };
        this.EditResponseAsync = context.EditResponseAsync;
        this.RespondTextAsync = context.CreateResponseAsync;
        this.DeferAsync = (bool e) =>
        { 
            IsDefered = true;
            return context.DeferAsync(e);
        };
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
        this.DeferAsync = async (bool e) => { return; }; //skipcq: CS-R1085
    }
}
