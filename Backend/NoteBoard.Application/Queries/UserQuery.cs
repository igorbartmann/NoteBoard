using System;
using Microsoft.EntityFrameworkCore;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Models.Note;
using NoteBoard.Application.Models.User;
using NoteBoard.Data.Persistence;
using NoteBoard.Domain.Entities;

namespace NoteBoard.Application.Queries
{
    public sealed class UserQuery : BaseQuery<User, UserCreateInputModel, UserUpdateInputModel, UserViewModel>, IUserQuery
    {
        public UserQuery(NoteBoardDbContext context, IUserMapper mapper) : base(context, mapper)
        {
            
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var query = (
                from u in _queryable
                where u.Email == email
                select 1
            );

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<UserViewModel?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var query = (
                from u in _queryable
                where u.Email == email
                select _mapper.ToViewModel(u)
            );

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<UserCredentialsValueObject?> GetUserCredentialsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var query = (
                from u in _queryable
                where u.Email == email
                select new UserCredentialsValueObject(
                    u.Id,
                    u.Name,
                    u.Email,
                    u.PasswordHash
                )
            );

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<UserWithNotesViewModel?> GetByIdWithNotesAsync(int id, CancellationToken cancellationToken)
        {
            var query = (
                from u in _queryable
                where u.Id == id
                select new UserWithNotesViewModel(
                    u.Id, 
                    u.Name, 
                    u.Email, 
                    u.CreatedAt, 
                    u.UpdatedAt, 
                    Notes: (
                        from n in u.Notes 
                        select new NoteViewModel(
                            n.Id, 
                            n.Title,
                            n.Content, 
                            n.IsCompleted,
                            n.Color, 
                            n.CreatedAt, 
                            n.CreatedBy, 
                            n.UpdatedAt)
                        ).ToList()
                )
            );

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}