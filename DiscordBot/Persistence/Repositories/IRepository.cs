namespace DiscordBot.Persistence.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(ulong id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(ulong id);
}
