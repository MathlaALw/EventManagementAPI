using System.Collections.Generic;
using System;

namespace EventManagementAPI.Repo
{
    public class GenericRepo<T> :  IGenericRepo<T> where T : class
    {
        // inject db context
        private readonly ApplicationDbContext _context;
       
        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        // CRUD operations
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public Task RemoveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
