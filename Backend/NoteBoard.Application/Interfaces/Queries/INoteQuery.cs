using System;
using NoteBoard.Application.Models.Note;

namespace NoteBoard.Application.Interfaces.Queries
{
    public interface INoteQuery : IBaseQuery<NoteViewModel>
    {
        Task<IEnumerable<NoteViewModel>> GetByOwnerIdAsync(int id, CancellationToken cancellationToken);

        Task<IEnumerable<NoteViewModel>> GetIncompleteByOwnerIdAsync(int id, CancellationToken cancellationToken);
    }
}