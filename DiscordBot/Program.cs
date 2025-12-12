using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Persistence.Repositories;
using DiscordBot.Services.CommandHandler;
using DiscordBot.Services.FileProcessing;
using DiscordBot.Services.InteractionHandler;
using DiscordBot.Services.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

public class Program
{
    private IServiceProvider _serviceProvider = null!;

    public static Task Main(string[] args) => new Program().RunAsync();

    public async Task RunAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        _serviceProvider = new ServiceCollection()
            .AddSingleton<PathService>()
            .AddSingleton<IDbService, DbService>()
            .AddSingleton<GuildRepository>()
            .AddSingleton<SoundboardSoundRepository>()
            .AddSingleton<IFileProcessingService, FileProcessingService>()
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.Guilds
            }))
            .AddSingleton<CommandService>()
            .AddSingleton(x => new InteractionService(
                x.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig { DefaultRunMode = Discord.Interactions.RunMode.Async }
            ))
            .AddSingleton<ILoggingService, LoggingService>()
            .AddSingleton<ITextCommandHandlerService, TextCommandHandlerService>()
            .AddSingleton<IInteractionHandlerService, InteractionHandlerService>()
            .BuildServiceProvider();


        var db = _serviceProvider.GetRequiredService<IDbService>();
        await db.ConnectAsync();

        _serviceProvider.GetRequiredService<ILoggingService>().Initialize(_serviceProvider);
        await _serviceProvider.GetRequiredService<ITextCommandHandlerService>().InitializeAsync();
        await _serviceProvider.GetRequiredService<IInteractionHandlerService>().InitializeAsync();

        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        await client.LoginAsync(TokenType.Bot, config["Secrets:BotToken"]);
        await client.StartAsync();

        var cts = new CancellationTokenSource();
        await Task.Delay(Timeout.Infinite, cts.Token);
    }
}
