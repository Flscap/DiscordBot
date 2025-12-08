namespace DiscordBot.Persistence.Repositories;

public interface IRepository<T>
{
    Task<T?> RetrieveByIdAsync(ulong id);
    Task<int> CreateAsync(T entity);
    Task<ulong> PersistAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(ulong id);
}
