using System;
using Microsoft.EntityFrameworkCore;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Models.Note;
using NoteBoard.Data.Persistence;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Queries
{
    public sealed class NoteQuery : BaseQuery<Note, NoteCreateInputModel, NoteUpdateInputModel, NoteViewModel>, INoteQuery
    {
        public NoteQuery(NoteBoardDbContext context, INoteMapper mapper) : base(context, mapper)
        {
            
        }

        public async Task<IEnumerable<NoteViewModel>> GetByOwnerIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = (
                from n in _queryable
                where n.CreatedBy == id
                select _mapper.ToViewModel(n)
            );

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<NoteViewModel>> GetIncompleteByOwnerIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = (
                from n in _queryable
                where n.CreatedBy == id
                    && !n.IsCompleted
                select _mapper.ToViewModel(n)
            );

            return await query.ToListAsync(cancellationToken);
        }
    }
}