﻿using DnKR.AmeliaBot.BotCommands.ChatCommands;
using DnKR.AmeliaBot;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;

public class ChatNextCommands : BaseCommandModule
{
    [Command("pasta"), Aliases("зфыеф", "паста")]
    public async Task SayPastaAsync(CommandContext ctx)
    {
        await ChatCommands.SayPastaAsync(new CommonContext(ctx));
    }

    [Command("coin"), Aliases("сщшт")]
    public async Task FlipCoinAsync(CommandContext ctx)
    {
        await ChatCommands.FlipCoinAsync(new CommonContext(ctx));
    }

    [Command("rand")]
    public async Task GetRandomAsync(CommandContext ctx, int minValue, int maxValue)
    {
        await ChatCommands.GetRandomAsync(new CommonContext(ctx), minValue, maxValue);
    }

    [Command("rand"), Aliases("кфтв")]
    public async Task GetRandomAsync(CommandContext ctx)
    {
        await ChatCommands.GetRandomAsync(new CommonContext(ctx));
    }
}
