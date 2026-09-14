using System;
using NoteBoard.Domain.Entities;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Data.Persistence.Repositories
{
    public sealed class UserRepository(NoteBoardDbContext context) : BaseRepository<User>(context), IUserRepository
    {
    }
}