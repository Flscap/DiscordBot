using System.Data;

namespace DiscordBot.Persistence.Repositories;

public interface IDbService : IAsyncDisposable
{
    IDbConnection Connection { get; }
    Task ConnectAsync();
}
