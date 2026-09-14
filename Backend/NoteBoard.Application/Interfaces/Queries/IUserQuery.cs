using System;
using NoteBoard.Application.Models.User;

namespace NoteBoard.Application.Interfaces.Queries
{
    public interface IUserQuery : IBaseQuery<UserViewModel>
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

        Task<UserViewModel?> GetByEmailAsync(string email, CancellationToken cancellationToken);

        Task<UserCredentialsValueObject?> GetUserCredentialsByEmailAsync(string email, CancellationToken cancellationToken);

        Task<UserWithNotesViewModel?> GetByIdWithNotesAsync(int id, CancellationToken cancellationToken);
    }
}