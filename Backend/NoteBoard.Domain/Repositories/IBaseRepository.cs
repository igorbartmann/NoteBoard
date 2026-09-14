using System;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Domain.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        Task<T?> ReadByIdAsync(int id, CancellationToken cancellationToken);
        void Update(T entity);
        void Remove(T entity);
    }
}