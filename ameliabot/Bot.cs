using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

using Lavalink4NET;

using DnKR.AmeliaBot.BotCommands.MusicCommands;
using DnKR.AmeliaBot.BotCommands.ChatCommands;
using DnKR.AmeliaBot.Music;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace DnKR.AmeliaBot;

public sealed class Bot : BackgroundService
{
    public static IAudioService AudioService { get; private set; }

    private readonly IServiceProvider serviceProvider;
    private readonly DiscordClient discordClient;

    public Bot(IServiceProvider serviceProvider, DiscordClient discordClient)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(discordClient);

        this.serviceProvider = serviceProvider;
        this.discordClient = discordClient;
        AudioService = serviceProvider.GetRequiredService<IAudioService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var slash = discordClient
            .UseSlashCommands(new SlashCommandsConfiguration { Services = serviceProvider });
        slash.RegisterCommands<MusicSlashCommands>();
        slash.RegisterCommands<ChatSlashCommands>();

        var next = discordClient
            .UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = ["d!", "в!"],
                Services = serviceProvider
            });
        next.RegisterCommands<ChatNextCommands>();
        next.RegisterCommands<MusicNextCommands>();


        discordClient.ComponentInteractionCreated += MusicEvents.ButtonSearchClicked;
        discordClient.VoiceStateUpdated += MusicEvents.VoiceStateUpdated;
        discordClient.MessageCreated += ChatEvents.MessageCreated;

        await discordClient
            .ConnectAsync()
            .ConfigureAwait(false);

        var readyTaskCompletionSource = new TaskCompletionSource();

        Task SetResult(DiscordClient client, ReadyEventArgs eventArgs)
        {
            readyTaskCompletionSource.TrySetResult();
            return Task.CompletedTask;
        }

        discordClient.Ready += SetResult;
        await readyTaskCompletionSource.Task.ConfigureAwait(false);
        discordClient.Ready -= SetResult;

        await Task
            .Delay(Timeout.InfiniteTimeSpan, stoppingToken)
            .ConfigureAwait(false);
    }
}
