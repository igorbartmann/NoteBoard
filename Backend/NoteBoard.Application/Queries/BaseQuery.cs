using System;
using Microsoft.EntityFrameworkCore;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Data.Persistence;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Interfaces.Queries
{
    public abstract class BaseQuery<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> : IBaseQuery<TViewModel> where TEntity : BaseEntity
    {
        protected readonly IQueryable<TEntity> _queryable;
        protected readonly IBaseMapper<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> _mapper;

        public BaseQuery(NoteBoardDbContext context, IBaseMapper<TEntity, TCreateInputModel, TUpdateInputModel, TViewModel> mapper)
        {
            _queryable = context.Set<TEntity>().AsNoTracking();
            _mapper = mapper;
        }

        public virtual async Task<TViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = (
                from e in _queryable
                where e.Id == id
                select _mapper.ToViewModel(e)
            );

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}