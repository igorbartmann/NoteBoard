using System;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Data.Persistence.Repositories
{
    public abstract class BaseRepository<T>(NoteBoardDbContext context) : IBaseRepository<T> where T : BaseEntity
    {
        private readonly NoteBoardDbContext _context = context;

        public async Task<T?> ReadByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}