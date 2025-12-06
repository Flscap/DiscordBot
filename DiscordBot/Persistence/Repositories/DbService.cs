using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DiscordBot.Persistence.Repositories;

public class DbService : IDbService
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Database not connected.");

    public DbService(string connectionString)
    {
        _connectionString = connectionString;
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

                CREATE TABLE IF NOT EXISTS Sounds (
                    Id INTEGER PRIMARY KEY,
                    Label TEXT NOT NULL,
                    Emoji TEXT,
                    ButtonStyle INTEGER NOT NULL,
                    GuildId INTEGER NOT NULL,
                    FOREIGN KEY (GuildId) REFERENCES Guild(Id) ON DELETE CASCADE
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
