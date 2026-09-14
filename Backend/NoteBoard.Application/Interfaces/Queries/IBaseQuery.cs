using System;

namespace NoteBoard.Application.Interfaces.Queries
{
    public interface IBaseQuery<TViewModel>
    {
        Task<TViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}