using System;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Data.Persistence.Repositories
{
    public sealed class NoteRepository(NoteBoardDbContext context) : BaseRepository<Note>(context), INoteRepository
    {
    }
}