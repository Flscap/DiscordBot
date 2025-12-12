using Dapper;
using DiscordBot.Services.FileProcessing;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DiscordBot.Persistence.Repositories;

public class DbService : IDbService
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Database not connected.");

    public DbService(PathService pathService)
    {
        _connectionString = $"Data Source={pathService.DatabasePath}";
    }

    public async Task ConnectAsync()
    {
        if(_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
            await _connection.OpenAsync();

            var initSql = @"
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS Guilds (
                    Id INTEGER NOT NULL PRIMARY KEY,
                    SoundboardTextChannelId INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS SoundboardSounds (
                    Id INTEGER PRIMARY KEY,
                    Label TEXT NOT NULL,
                    Emoji TEXT,
                    ButtonStyle INTEGER NOT NULL,
                    FilePath TEXT NOT NULL,
                    GuildId INTEGER NOT NULL,
                    FOREIGN KEY (GuildId) REFERENCES Guilds(Id) ON DELETE CASCADE
                );";

            await _connection.ExecuteAsync(initSql);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
