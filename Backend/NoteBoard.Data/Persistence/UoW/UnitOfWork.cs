using System;

namespace NoteBoard.Data.Persistence.UoW
{
    public sealed class UnitOfWork(NoteBoardDbContext context) : IUnitOfWork
    {
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken); 
        }
    }
}