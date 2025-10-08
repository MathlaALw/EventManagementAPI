
namespace EventManagementAPI.Repo
{
    public interface IGenericRepo<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T?> GetByIdAsync(Guid id);
        Task RemoveAsync(T entity);
        Task SaveChangesAsync();
    }
}