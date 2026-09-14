using System;

namespace NoteBoard.Data.Persistence.UoW
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken cancellationToken);
    }
}